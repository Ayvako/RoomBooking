namespace RoomBooking.Application.Services;

using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Enums;

public class BookingApplicationService(IBookingRepository bookingRepository)
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return bookingRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return bookingRepository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        ValidatePeriod(booking);

        var existingBookings =
            await bookingRepository.GetByRoomAndPeriodAsync(
                booking.RoomId,
                booking.StartTime,
                booking.EndTime,
                cancellationToken);

        var hasConflict = existingBookings.Any(existing =>
            existing.Status == BookingStatus.Active &&
            booking.StartTime < existing.EndTime &&
            booking.EndTime > existing.StartTime);

        if (hasConflict)
        {
            throw new InvalidOperationException("The room is already booked for the selected period.");
        }

        booking.Status = BookingStatus.Active;

        await bookingRepository.AddAsync(booking, cancellationToken);
    }

    public Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        ValidatePeriod(booking);

        return bookingRepository.UpdateAsync(booking, cancellationToken);
    }

    public Task DeleteAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        return bookingRepository.DeleteAsync(booking, cancellationToken);
    }

    public async Task CancelAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        booking.Status = BookingStatus.Cancelled;

        await bookingRepository.UpdateAsync(booking, cancellationToken);
    }

    private static void ValidatePeriod(Booking booking)
    {
        if (booking.StartTime >= booking.EndTime)
        {
            throw new ArgumentException("Start time must be earlier than end time.");
        }
    }
}