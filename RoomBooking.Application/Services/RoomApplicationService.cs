namespace RoomBooking.Application.Services;

using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

public class RoomApplicationService(IRoomRepository roomRepository)
{
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return roomRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return roomRepository.GetAllAsync(cancellationToken);
    }

    public Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        return roomRepository.AddAsync(room, cancellationToken);
    }

    public Task UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        return roomRepository.UpdateAsync(room, cancellationToken);
    }

    public Task DeleteAsync(Room room, CancellationToken cancellationToken = default)
    {
        return roomRepository.DeleteAsync(room, cancellationToken);
    }
}