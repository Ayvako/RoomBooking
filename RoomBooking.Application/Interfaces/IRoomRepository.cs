using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Room room, CancellationToken cancellationToken = default);

    Task UpdateAsync(Room room, CancellationToken cancellationToken = default);

    Task DeleteAsync(Room room, CancellationToken cancellationToken = default);
}