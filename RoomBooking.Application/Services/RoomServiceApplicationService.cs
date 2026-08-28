namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

public class RoomServiceApplicationService(IRoomServiceRepository roomServiceRepository, IRoomRepository roomRepository)
{
    public async Task<RoomServiceResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await roomServiceRepository.GetByIdAsync(id, cancellationToken);

        return service is null ? null : MapToResponse(service);
    }

    public async Task<IReadOnlyList<RoomServiceResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var services = await roomServiceRepository.GetAllAsync(cancellationToken);

        return [.. services.Select(MapToResponse)];
    }

    public async Task<IReadOnlyList<RoomServiceResponse>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithServicesAsync(roomId, cancellationToken);

        if (room is null)
        {
            return [];
        }

        return [.. room.Services.Select(MapToResponse)];
    }

    public async Task<bool> AddToRoomAsync(Guid roomId, Guid serviceId, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithServicesAsync(roomId, cancellationToken);

        if (room is null)
        {
            return false;
        }

        var service = await roomServiceRepository.GetByIdWithRoomsAsync(serviceId, cancellationToken);

        if (service is null)
        {
            return false;
        }

        if (service.Rooms.All(x => x.Id != roomId))
        {
            room.Services.Add(service);

            await roomRepository.UpdateAsync(room, cancellationToken);
        }

        return true;
    }

    public async Task<RoomServiceResponse> AddAsync(CreateRoomServiceRequest request, CancellationToken cancellationToken = default)
    {
        var service = new RoomService
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
        };

        await roomServiceRepository.AddAsync(service, cancellationToken);

        return MapToResponse(service);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRoomServiceRequest request, CancellationToken cancellationToken = default)
    {
        var service = await roomServiceRepository.GetByIdAsync(id, cancellationToken);

        if (service is null)
        {
            return false;
        }

        service.Name = request.Name;
        service.Price = request.Price;

        await roomServiceRepository.UpdateAsync(service, cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await roomServiceRepository.GetByIdWithRoomsAsync(id, cancellationToken);

        if (service is null)
        {
            return false;
        }

        service.Rooms.Clear();

        await roomServiceRepository.DeleteAsync(service, cancellationToken);

        return true;
    }

    private static RoomServiceResponse MapToResponse(RoomService service)
    {
        return new RoomServiceResponse
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
        };
    }
}