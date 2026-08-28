namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

/// <summary>
/// Provides application-level operations for managing room services
/// and their relationships with rooms.
/// </summary>
public class RoomServiceApplicationService(IRoomServiceRepository roomServiceRepository, IRoomRepository roomRepository)
{
    /// <summary>
    /// Gets a room service by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the room service.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// The room service response if the service exists; otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<RoomServiceResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await roomServiceRepository.GetByIdAsync(id, cancellationToken);

        return service is null ? null : MapToResponse(service);
    }

    /// <summary>
    /// Gets all room services.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A collection of room service responses.</returns>
    public async Task<IReadOnlyList<RoomServiceResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var services = await roomServiceRepository.GetAllAsync(cancellationToken);

        return [.. services.Select(MapToResponse)];
    }

    /// <summary>
    /// Gets all services assigned to a specific room.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// A collection of room service responses assigned to the room.
    /// Returns an empty collection if the room does not exist.
    /// </returns>
    public async Task<IReadOnlyList<RoomServiceResponse>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithServicesAsync(roomId, cancellationToken);

        if (room is null)
        {
            return [];
        }

        return [.. room.Services.Select(MapToResponse)];
    }

    /// <summary>
    /// Assigns a room service to a room.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="serviceId">The identifier of the room service.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if both the room and service exist;
    /// otherwise, <see langword="false"/>.
    /// </returns>
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

    /// <summary>
    /// Creates a new room service.
    /// </summary>
    /// <param name="request">The request containing the room service data.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The created room service response.</returns>
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

    /// <summary>
    /// Updates an existing room service.
    /// </summary>
    /// <param name="id">The identifier of the room service.</param>
    /// <param name="request">The request containing the updated room service data.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if the room service was found and updated;
    /// otherwise, <see langword="false"/>.
    /// </returns>
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

    /// <summary>
    /// Deletes a room service and removes its room relationships.
    /// </summary>
    /// <param name="id">The identifier of the room service.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if the room service was found and deleted;
    /// otherwise, <see langword="false"/>.
    /// </returns>
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

    /// <summary>
    /// Maps a room service entity to its response DTO.
    /// </summary>
    /// <param name="service">The room service entity.</param>
    /// <returns>The mapped room service response.</returns>
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