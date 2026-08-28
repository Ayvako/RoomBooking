namespace RoomBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.DTOs.Reports;
using RoomBooking.Application.Services;

[ApiController]
[Route("api/reports")]
public class ReportsController(ReportApplicationService reportApplicationService) : ControllerBase
{
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
}