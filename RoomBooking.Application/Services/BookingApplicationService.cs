namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.Bookings;
using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Enums;

public class BookingApplicationService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
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

        await this.EnsureNoConflictAsync(request.RoomId, request.StartTime, request.EndTime, cancellationToken);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPrice = CalculateTotalPrice(room, request.StartTime, request.EndTime),
            Status = BookingStatus.Active,
        };

        await bookingRepository.AddAsync(booking, cancellationToken);

        booking.Room = room;

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

        booking.TotalPrice = CalculateTotalPrice(booking.Room, request.StartTime, request.EndTime);

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

    private static decimal CalculateTotalPrice(Room room, DateTime startTime, DateTime endTime)
    {
        var duration = endTime - startTime;
        var hours = (decimal)duration.TotalHours;

        var servicesPrice = room.Services.Sum(
            service => service.Price);

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
}