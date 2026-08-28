namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.Bookings;
using RoomBooking.Application.Services;

[ApiController]
[Route("api/bookings")]
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
        var booking = await bookingApplicationService.AddAsync(request, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = booking.Id }, booking);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        var updated = await bookingApplicationService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
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