namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Services;

[ApiController]
[Route("api/rooms/{roomId:guid}/services")]
public class RoomServicesController(RoomServiceApplicationService roomServiceApplicationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomServiceResponse>>> GetByRoomId(Guid roomId, CancellationToken cancellationToken)
    {
        var services = await roomServiceApplicationService.GetByRoomIdAsync(roomId, cancellationToken);

        return this.Ok(services);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomServiceResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var service = await roomServiceApplicationService.GetByIdAsync(id, cancellationToken);

        if (service is null)
        {
            return this.NotFound();
        }

        return this.Ok(service);
    }

    [HttpPost]
    public async Task<ActionResult<RoomServiceResponse>> Create(Guid roomId, CreateRoomServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await roomServiceApplicationService.AddAsync(roomId, request, cancellationToken);

        if (service is null)
        {
            return this.NotFound();
        }

        return this.CreatedAtAction(nameof(this.GetById), new { roomId, id = service.Id }, service);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoomServiceRequest request, CancellationToken cancellationToken)
    {
        var updated = await roomServiceApplicationService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await roomServiceApplicationService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}