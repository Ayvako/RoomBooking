namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.Reports;
using RoomBooking.Application.Services;

/// <summary>
/// Provides endpoints for generating booking and room statistics.
/// </summary>
[ApiController]
[Route("api/reports")]
public class ReportsController(ReportApplicationService reportApplicationService) : ControllerBase
{
    /// <summary>
    /// Gets booking statistics for the specified period.
    /// </summary>
    /// <param name="from">The start of the reporting period.</param>
    /// <param name="to">The end of the reporting period.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>Booking statistics for the specified period.</returns>
    [HttpGet("bookings")]
    public async Task<ActionResult<BookingStatisticsResponse>> GetBookingStatistics([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken)
    {
        if (from >= to)
        {
            return this.BadRequest(new
            {
                status = 400,
                message = "The start date must be earlier than the end date.",
            });
        }

        var statistics = await reportApplicationService.GetBookingStatisticsAsync(from, to, cancellationToken);

        return this.Ok(statistics);
    }

    /// <summary>
    /// Gets booking statistics grouped by room for the specified period.
    /// </summary>
    /// <param name="from">The start of the reporting period.</param>
    /// <param name="to">The end of the reporting period.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>Booking statistics grouped by room for the specified period.</returns>
    [HttpGet("rooms")]
    public async Task<ActionResult<IReadOnlyList<RoomBookingStatisticsResponse>>> GetRoomBookingStatistics([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken)
    {
        if (from >= to)
        {
            return this.BadRequest(new
            {
                status = 400,
                message = "The start date must be earlier than the end date.",
            });
        }

        var statistics = await reportApplicationService.GetRoomBookingStatisticsAsync(from, to, cancellationToken);

        return this.Ok(statistics);
    }
}