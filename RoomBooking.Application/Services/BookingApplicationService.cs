namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.Bookings;
using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Enums;

/// <summary>
/// Provides application-level operations for managing bookings.
/// </summary>
public class BookingApplicationService(IBookingRepository bookingRepository, IRoomRepository roomRepository, IRoomServiceRepository roomServiceRepository, IPricingService pricingService)
{
    /// <summary>
    /// Gets a booking by its identifier.
    /// </summary>
    /// <param name="id">The booking identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    /// The booking response if found; otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<BookingResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(id, cancellationToken);

        return booking is null ? null : MapToResponse(booking);
    }

    /// <summary>
    /// Gets all bookings.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A collection of booking responses.</returns>
    public async Task<IReadOnlyList<BookingResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository.GetAllAsync(cancellationToken);

        return [.. bookings.Select(MapToResponse)];
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="request">The booking creation request.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The created booking.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the room identifier is empty or the booking period is invalid.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the requested room or room service does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the room is already booked for the selected period.
    /// </exception>
    public async Task<BookingResponse> AddAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RoomId == Guid.Empty)
        {
            throw new ArgumentException("Room ID cannot be empty.");
        }

        ValidatePeriod(request.StartTime, request.EndTime);

        var room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken) ?? throw new KeyNotFoundException($"Room with id '{request.RoomId}' was not found.");

        var services = await this.GetSelectedServicesAsync(room.Id, request.ServiceIds, cancellationToken);

        await this.EnsureNoConflictAsync(request.RoomId, request.StartTime, request.EndTime, cancellationToken);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            Room = room,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPrice = pricingService.Calculate(
                room,
                services,
                request.StartTime,
                request.EndTime),
            Status = BookingStatus.Active,
            Services = services,
        };

        await bookingRepository.AddAsync(booking, cancellationToken);

        return MapToResponse(booking);
    }

    /// <summary>
    /// Updates the period of an existing booking.
    /// </summary>
    /// <param name="id">The booking identifier.</param>
    /// <param name="request">The booking update request.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if the booking was updated;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the booking period is invalid.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the booking is cancelled or the room is already booked
    /// for the selected period.
    /// </exception>
    public async Task<bool> UpdateAsync(Guid id, UpdateBookingRequest request, CancellationToken cancellationToken = default)
    {
        ValidatePeriod(request.StartTime, request.EndTime);

        var booking = await bookingRepository.GetByIdAsync(id, cancellationToken);

        if (booking is null)
        {
            return false;
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled booking cannot be updated.");
        }

        await this.EnsureNoConflictAsync(booking.RoomId, request.StartTime, request.EndTime, cancellationToken, booking.Id);

        booking.StartTime = request.StartTime;
        booking.EndTime = request.EndTime;

        booking.TotalPrice = pricingService.Calculate(booking.Room, booking.Services, request.StartTime, request.EndTime);

        await bookingRepository.UpdateAsync(booking, cancellationToken);

        return true;
    }

    /// <summary>
    /// Cancels an existing booking.
    /// </summary>
    /// <param name="id">The booking identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if the booking exists and was cancelled;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(id, cancellationToken);

        if (booking is null)
        {
            return false;
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return true;
        }

        booking.Status = BookingStatus.Cancelled;

        await bookingRepository.UpdateAsync(booking, cancellationToken);

        return true;
    }

    private static void ValidatePeriod(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be earlier than end time.");
        }
    }

    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            Room = new RoomResponse
            {
                Id = booking.Room.Id,
                Name = booking.Room.Name,
                Capacity = booking.Room.Capacity,
                BaseHourlyRate = booking.Room.BaseHourlyRate,
                Services = [.. booking.Room.Services
                    .Select(service => new RoomServiceResponse
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Price = service.Price,
                    })],
            },
        };
    }

    private async Task EnsureNoConflictAsync(Guid roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken, Guid? excludedBookingId = null)
    {
        var bookings = await bookingRepository.GetByRoomAndPeriodAsync(roomId, startTime, endTime, cancellationToken);

        var hasConflict = bookings.Any(booking =>
            booking.Status == BookingStatus.Active &&
            booking.Id != excludedBookingId &&
            startTime < booking.EndTime &&
            endTime > booking.StartTime);

        if (hasConflict)
        {
            throw new InvalidOperationException("The room is already booked for the selected period.");
        }
    }

    private async Task<List<RoomService>> GetSelectedServicesAsync(Guid roomId, IReadOnlyList<Guid> serviceIds, CancellationToken cancellationToken)
    {
        if (serviceIds.Count == 0)
        {
            return [];
        }

        var services = new List<RoomService>();

        foreach (var serviceId in serviceIds.Distinct())
        {
            var service = await roomServiceRepository.GetByIdWithRoomsAsync(serviceId, cancellationToken) ??
                throw new KeyNotFoundException($"Room service with id '{serviceId}' was not found.");

            if (!service.Rooms.Any(room => room.Id == roomId))
            {
                throw new ArgumentException($"Room service '{serviceId}' does not belong to room '{roomId}'.");
            }

            services.Add(service);
        }

        return services;
    }
}