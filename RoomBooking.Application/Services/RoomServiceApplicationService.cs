namespace RoomBooking.Application.Services;

using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

public class RoomServiceApplicationService(IRoomServiceRepository roomServiceRepository)
{
    public Task<RoomService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return roomServiceRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<RoomService>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return roomServiceRepository.GetByRoomIdAsync(roomId, cancellationToken);
    }

    public Task AddAsync(RoomService service, CancellationToken cancellationToken = default)
    {
        return roomServiceRepository.AddAsync(service, cancellationToken);
    }

    public Task UpdateAsync(RoomService service, CancellationToken cancellationToken = default)
    {
        return roomServiceRepository.UpdateAsync(service, cancellationToken);
    }

    public Task DeleteAsync(RoomService service, CancellationToken cancellationToken = default)
    {
        return roomServiceRepository.DeleteAsync(service, cancellationToken);
    }
}