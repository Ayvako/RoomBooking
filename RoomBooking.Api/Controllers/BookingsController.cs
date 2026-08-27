namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.Bookings;
using RoomBooking.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(BookingApplicationService bookingApplicationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var bookings = await bookingApplicationService.GetAllAsync(cancellationToken);

        return this.Ok(bookings);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var booking = await bookingApplicationService.GetByIdAsync(id, cancellationToken);

        if (booking is null)
        {
            return this.NotFound();
        }

        return this.Ok(booking);
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var booking = await bookingApplicationService.AddAsync(request, cancellationToken);

            return this.CreatedAtAction(nameof(this.GetById), new { id = booking.Id }, booking);
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
        catch (ArgumentException)
        {
            return this.BadRequest();
        }
        catch (InvalidOperationException)
        {
            return this.Conflict();
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await bookingApplicationService.UpdateAsync(id, request, cancellationToken);

            if (!updated)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
        catch (ArgumentException)
        {
            return this.BadRequest();
        }
        catch (InvalidOperationException)
        {
            return this.Conflict();
        }
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var cancelled = await bookingApplicationService.CancelAsync(id, cancellationToken);

        if (!cancelled)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}