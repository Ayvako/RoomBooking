namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Application.Services;

/// <summary>
/// Provides endpoints for managing rooms.
/// </summary>
[ApiController]
[Route("api/rooms")]
public class RoomsController(RoomApplicationService roomApplicationService) : ControllerBase
{
    /// <summary>
    /// Gets all rooms.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of all rooms.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await roomApplicationService.GetAllAsync(cancellationToken);

        return this.Ok(rooms);
    }

    /// <summary>
    /// Gets a room by its identifier.
    /// </summary>
    /// <param name="id">The room identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The requested room, or a 404 response if it does not exist.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var room = await roomApplicationService.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return this.NotFound();
        }

        return this.Ok(room);
    }

    /// <summary>
    /// Creates a new room.
    /// </summary>
    /// <param name="request">The room creation data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The newly created room.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoomResponse>> Create(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await roomApplicationService.AddAsync(request, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = room.Id }, room);
    }

    /// <summary>
    /// Updates an existing room.
    /// </summary>
    /// <param name="id">The room identifier.</param>
    /// <param name="request">The updated room data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 204 response if the room was updated, or a 404 response if it does not exist.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var updated = await roomApplicationService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Deletes a room.
    /// </summary>
    /// <param name="id">The room identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 204 response if the room was deleted, or a 404 response if it does not exist.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await roomApplicationService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Gets rooms available for the specified time period and capacity.
    /// </summary>
    /// <param name="request">The room availability search criteria.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of rooms available for the specified criteria.</returns>
    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAvailable([FromQuery] AvailableRoomsRequest request, CancellationToken cancellationToken)
    {
        var rooms = await roomApplicationService.GetAvailableAsync(request, cancellationToken);

        return this.Ok(rooms);
    }
}