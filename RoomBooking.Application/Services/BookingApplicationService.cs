namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.Bookings;
using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Enums;

public class BookingApplicationService(IBookingRepository bookingRepository, IRoomRepository roomRepository, IRoomServiceRepository roomServiceRepository)
{
    public async Task<BookingResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(id, cancellationToken);

        return booking is null ? null : MapToResponse(booking);
    }

    public async Task<IReadOnlyList<BookingResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository.GetAllAsync(cancellationToken);

        return [.. bookings.Select(MapToResponse)];
    }

    public async Task<BookingResponse> AddAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
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
            TotalPrice = CalculateTotalPrice(
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

        booking.TotalPrice = CalculateTotalPrice(booking.Room, booking.Services, request.StartTime, request.EndTime);

        await bookingRepository.UpdateAsync(booking, cancellationToken);

        return true;
    }

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

    private static decimal CalculateTotalPrice(Room room, IEnumerable<RoomService> services, DateTime startTime, DateTime endTime)
    {
        var hours = (decimal)(endTime - startTime).TotalHours;

        var servicesPrice = services.Sum(service => service.Price);

        return (room.BaseHourlyRate * hours) + servicesPrice;
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
            Room = booking.Room is null
                ? null
                : new RoomResponse
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
            var service = await roomServiceRepository.GetByIdAsync(serviceId, cancellationToken) ?? throw new KeyNotFoundException($"Room service with id '{serviceId}' was not found.");

            if (service.RoomId != roomId)
            {
                throw new ArgumentException($"Room service '{serviceId}' does not belong to room '{roomId}'.");
            }

            services.Add(service);
        }

        return services;
    }
}