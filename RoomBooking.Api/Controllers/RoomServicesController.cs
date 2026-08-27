namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Services;

[ApiController]
[Route("api/rooms/{roomId:guid}/services")]
public class RoomServicesController(RoomServiceApplicationService roomServiceApplicationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByRoomId(Guid roomId, CancellationToken cancellationToken)
    {
        var services = await roomServiceApplicationService.GetByRoomIdAsync(roomId, cancellationToken);

        return this.Ok(services);
    }

    [HttpPost("{serviceId:guid}")]
    public async Task<IActionResult> AddToRoom(Guid roomId, Guid serviceId, CancellationToken cancellationToken)
    {
        var result =
            await roomServiceApplicationService
                .AddToRoomAsync(
                    roomId,
                    serviceId,
                    cancellationToken);

        if (!result)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}