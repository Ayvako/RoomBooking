namespace RoomBooking.Application.Interfaces;

using RoomBooking.Domain.Entities;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Room room, CancellationToken cancellationToken = default);

    Task UpdateAsync(Room room, CancellationToken cancellationToken = default);

    Task DeleteAsync(Room room, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Room>> GetAvailableAsync(DateTime startTime, DateTime endTime, int capacity, CancellationToken cancellationToken = default);
}