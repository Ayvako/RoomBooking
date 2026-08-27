namespace RoomBooking.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;
using RoomBooking.Infrastructure.Persistence;

public class BookingRepository(RoomBookingDbContext context) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Bookings
            .Include(booking => booking.Room)
            .ThenInclude(room => room.Services)
            .FirstOrDefaultAsync(
                booking => booking.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Bookings
            .Include(booking => booking.Room)
            .ThenInclude(room => room.Services)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByRoomAndPeriodAsync(Guid roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await context.Bookings
            .Where(booking =>
                booking.RoomId == roomId &&
                booking.StartTime < endTime &&
                booking.EndTime > startTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await context.Bookings.AddAsync(booking, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        context.Bookings.Update(booking);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        context.Bookings.Remove(booking);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return await context.Bookings.AnyAsync(x => x.RoomId == roomId, cancellationToken);
    }
}