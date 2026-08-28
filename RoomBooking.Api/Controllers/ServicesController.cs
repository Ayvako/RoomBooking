namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Services;

/// <summary>
/// Provides endpoints for managing the room service catalog.
/// </summary>
[ApiController]
[Route("api/services")]
public class ServicesController(RoomServiceApplicationService roomServiceApplicationService) : ControllerBase
{
    /// <summary>
    /// Gets all available room services.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of all available room services.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomServiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var services = await roomServiceApplicationService.GetAllAsync(cancellationToken);

        return this.Ok(services);
    }

    /// <summary>
    /// Gets a room service by its identifier.
    /// </summary>
    /// <param name="id">The service identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The requested room service, or a 404 response if it does not exist.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoomServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomServiceResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var service = await roomServiceApplicationService.GetByIdAsync(id, cancellationToken);

        if (service is null)
        {
            return this.NotFound();
        }

        return this.Ok(service);
    }

    /// <summary>
    /// Creates a new room service.
    /// </summary>
    /// <param name="request">The service creation data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The newly created room service.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RoomServiceResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoomServiceResponse>> Create(CreateRoomServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await roomServiceApplicationService.AddAsync(request, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = service.Id }, service);
    }

    /// <summary>
    /// Updates an existing room service.
    /// </summary>
    /// <param name="id">The service identifier.</param>
    /// <param name="request">The updated service data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 204 response if the service was updated, or a 404 response if it does not exist.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateRoomServiceRequest request, CancellationToken cancellationToken)
    {
        var updated = await roomServiceApplicationService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Deletes a room service.
    /// </summary>
    /// <param name="id">The service identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 204 response if the service was deleted, or a 404 response if it does not exist.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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