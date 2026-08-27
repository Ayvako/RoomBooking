namespace RoomBooking.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;
using RoomBooking.Domain.Enums;
using RoomBooking.Infrastructure.Persistence;

public class RoomRepository(RoomBookingDbContext context) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Rooms
            .FirstOrDefaultAsync(
                room => room.Id == id,
                cancellationToken);
    }

    public async Task<Room?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Rooms
            .Include(room => room.Services)
            .FirstOrDefaultAsync(
                room => room.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Rooms
            .Include(room => room.Services)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await context.Rooms.AddAsync(room, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        context.Rooms.Update(room);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Room room, CancellationToken cancellationToken = default)
    {
        context.Rooms.Remove(room);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Room>> GetAvailableAsync(DateTime startTime, DateTime endTime, int capacity, CancellationToken cancellationToken = default)
    {
        return await context.Rooms
            .Include(room => room.Services)
            .Where(room => room.Capacity >= capacity)
            .Where(room =>
                !room.Bookings.Any(booking =>
                    booking.Status == BookingStatus.Active &&
                    startTime < booking.EndTime &&
                    endTime > booking.StartTime))
            .ToListAsync(cancellationToken);
    }
}