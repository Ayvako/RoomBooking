namespace RoomBooking.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;
using RoomBooking.Infrastructure.Persistence;

public class RoomServiceRepository(RoomBookingDbContext context) : IRoomServiceRepository
{
    public async Task<RoomService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.RoomServices
            .FirstOrDefaultAsync(
                service => service.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(RoomService service, CancellationToken cancellationToken = default)
    {
        await context.RoomServices.AddAsync(
            service,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(RoomService service, CancellationToken cancellationToken = default)
    {
        context.RoomServices.Update(service);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RoomService service, CancellationToken cancellationToken = default)
    {
        context.RoomServices.Remove(service);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RoomService?> GetByIdWithRoomsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.RoomServices
            .Include(service => service.Rooms)
            .FirstOrDefaultAsync(
                service => service.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<RoomService>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.RoomServices
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}