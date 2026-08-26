namespace RoomBooking.Application.Interfaces;

using RoomBooking.Domain.Entities;

public interface IRoomServiceRepository
{
    Task<RoomService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoomService>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);

    Task AddAsync(RoomService service, CancellationToken cancellationToken = default);

    Task UpdateAsync(RoomService service, CancellationToken cancellationToken = default);

    Task DeleteAsync(RoomService service, CancellationToken cancellationToken = default);
}