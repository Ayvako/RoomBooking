namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

public class RoomApplicationService(IRoomRepository roomRepository)
{
    public async Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await roomRepository.GetAllAsync(cancellationToken);

        return [.. rooms.Select(MapToResponse)];
    }

    public async Task<RoomResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(id, cancellationToken);

        return room is null ? null : MapToResponse(room);
    }

    public async Task<RoomResponse> AddAsync(CreateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Capacity = request.Capacity,
            BaseHourlyRate = request.BaseHourlyRate,
        };

        await roomRepository.AddAsync(room, cancellationToken);

        return MapToResponse(room);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (room is null)
        {
            return false;
        }

        room.Name = request.Name;
        room.Capacity = request.Capacity;
        room.BaseHourlyRate = request.BaseHourlyRate;

        await roomRepository.UpdateAsync(room, cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return false;
        }

        await roomRepository.DeleteAsync(room, cancellationToken);

        return true;
    }

    private static RoomResponse MapToResponse(Room room)
    {
        return new RoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            BaseHourlyRate = room.BaseHourlyRate,
            Services = [.. room.Services
                .Select(service => new RoomServiceResponse
                {
                    Id = service.Id,
                    Name = service.Name,
                    Price = service.Price,
                })],
        };
    }
}