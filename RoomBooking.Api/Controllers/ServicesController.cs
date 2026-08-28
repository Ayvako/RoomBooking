namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.RoomServices;
using RoomBooking.Application.Services;

[ApiController]
[Route("api/services")]
public class ServicesController(RoomServiceApplicationService roomServiceApplicationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomServiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var services = await roomServiceApplicationService.GetAllAsync(cancellationToken);

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
    [ProducesResponseType(typeof(RoomServiceResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoomServiceResponse>> Create(CreateRoomServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await roomServiceApplicationService.AddAsync(request, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = service.Id }, service);
    }

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