namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Services;

/// <summary>
/// Provides endpoints for managing services assigned to rooms.
/// </summary>
[ApiController]
[Route("api/rooms/{roomId:guid}/services")]
public class RoomServicesController(RoomServiceApplicationService roomServiceApplicationService) : ControllerBase
{
    /// <summary>
    /// Gets all services assigned to a room.
    /// </summary>
    /// <param name="roomId">The room identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of services assigned to the room.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoomServiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByRoomId(Guid roomId, CancellationToken cancellationToken)
    {
        var services = await roomServiceApplicationService.GetByRoomIdAsync(roomId, cancellationToken);

        return this.Ok(services);
    }

    /// <summary>
    /// Assigns a service to a room.
    /// </summary>
    /// <param name="roomId">The room identifier.</param>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 204 response if the service was assigned, or a 404 response if the room or service does not exist.</returns>
    [HttpPost("{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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