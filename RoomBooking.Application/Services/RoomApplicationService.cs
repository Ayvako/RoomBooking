namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

/// <summary>
/// Provides application-level operations for managing rooms.
/// </summary>
public class RoomApplicationService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
{
    /// <summary>
    /// Gets all rooms.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A collection of room responses.</returns>
    public async Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await roomRepository.GetAllAsync(cancellationToken);

        return [.. rooms.Select(MapToResponse)];
    }

    /// <summary>
    /// Gets a room by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the room.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// The room response if the room exists; otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<RoomResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(id, cancellationToken);

        return room is null ? null : MapToResponse(room);
    }

    /// <summary>
    /// Creates a new room.
    /// </summary>
    /// <param name="request">The request containing the room data.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The created room response.</returns>
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

    /// <summary>
    /// Updates an existing room.
    /// </summary>
    /// <param name="id">The identifier of the room.</param>
    /// <param name="request">The request containing the updated room data.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if the room was found and updated;
    /// otherwise, <see langword="false"/>.
    /// </returns>
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

    /// <summary>
    /// Deletes a room if it has no associated bookings.
    /// </summary>
    /// <param name="id">The identifier of the room.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if the room was found and deleted;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the room has existing bookings.
    /// </exception>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithServicesAsync(id, cancellationToken);

        if (room is null)
        {
            return false;
        }

        var hasBookings = await bookingRepository.ExistsByRoomIdAsync(id, cancellationToken);

        if (hasBookings)
        {
            throw new InvalidOperationException("Room cannot be deleted because it has bookings.");
        }

        room.Services.Clear();

        await roomRepository.DeleteAsync(room, cancellationToken);

        return true;
    }

    /// <summary>
    /// Gets rooms that are available for the specified period and capacity.
    /// </summary>
    /// <param name="request">
    /// The request containing the desired time period and minimum capacity.
    /// </param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A collection of available room responses.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the start time is not earlier than the end time.
    /// </exception>
    public async Task<IReadOnlyList<RoomResponse>> GetAvailableAsync(AvailableRoomsRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StartTime >= request.EndTime)
        {
            throw new ArgumentException(
                "Start time must be earlier than end time.");
        }

        var rooms = await roomRepository.GetAvailableAsync(request.StartTime, request.EndTime, request.Capacity, cancellationToken);

        return [.. rooms.Select(MapToResponse)];
    }

    /// <summary>
    /// Maps a room entity to its response DTO.
    /// </summary>
    /// <param name="room">The room entity.</param>
    /// <returns>The mapped room response.</returns>
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