namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Application.Services;

[ApiController]
[Route("api/rooms")]
public class RoomsController(RoomApplicationService roomApplicationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await roomApplicationService.GetAllAsync(cancellationToken);

        return this.Ok(rooms);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var room = await roomApplicationService.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return this.NotFound();
        }

        return this.Ok(room);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoomResponse>> Create(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await roomApplicationService.AddAsync(request, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = room.Id }, room);
    }

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

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAvailable([FromQuery] AvailableRoomsRequest request, CancellationToken cancellationToken)
    {
        var rooms = await roomApplicationService.GetAvailableAsync(request, cancellationToken);

        return this.Ok(rooms);
    }
}