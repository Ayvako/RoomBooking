namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.Bookings;
using RoomBooking.Application.Services;

/// <summary>
/// Provides endpoints for managing room bookings.
/// </summary>
[ApiController]
[Route("api/bookings")]
public class BookingsController(BookingApplicationService bookingApplicationService) : ControllerBase
{
    /// <summary>
    /// Gets all bookings.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of all bookings.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var bookings = await bookingApplicationService.GetAllAsync(cancellationToken);

        return this.Ok(bookings);
    }

    /// <summary>
    /// Gets a booking by its identifier.
    /// </summary>
    /// <param name="id">The booking identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The requested booking, or a 404 response if it does not exist.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var booking = await bookingApplicationService.GetByIdAsync(id, cancellationToken);

        if (booking is null)
        {
            return this.NotFound();
        }

        return this.Ok(booking);
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="request">The booking creation data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The newly created booking.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<BookingResponse>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await bookingApplicationService.AddAsync(request, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = booking.Id }, booking);
    }

    /// <summary>
    /// Updates the time period of an existing booking.
    /// </summary>
    /// <param name="id">The booking identifier.</param>
    /// <param name="request">The updated booking period.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 204 response if the booking was updated, or a 404 response if it does not exist.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        var updated = await bookingApplicationService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Cancels an existing booking.
    /// </summary>
    /// <param name="id">The booking identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 204 response if the booking was cancelled, or a 404 response if it does not exist.</returns>
    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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