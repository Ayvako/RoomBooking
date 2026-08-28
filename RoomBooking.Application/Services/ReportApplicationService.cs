namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.Reports;
using RoomBooking.Application.Interfaces;

/// <summary>
/// Provides application-level operations for generating booking reports.
/// </summary>
public class ReportApplicationService(IReportRepository reportRepository)
{
    /// <summary>
    /// Gets aggregated booking statistics for the specified period.
    /// </summary>
    /// <param name="from">The start of the reporting period.</param>
    /// <param name="to">The end of the reporting period.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Aggregated booking statistics for the specified period.</returns>
    public async Task<BookingStatisticsResponse> GetBookingStatisticsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await reportRepository.GetBookingStatisticsAsync(from, to, cancellationToken);
    }

    /// <summary>
    /// Gets booking statistics grouped by room for the specified period.
    /// </summary>
    /// <param name="from">The start of the reporting period.</param>
    /// <param name="to">The end of the reporting period.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    /// A collection of booking statistics grouped by room.
    /// </returns>
    public async Task<IReadOnlyList<RoomBookingStatisticsResponse>> GetRoomBookingStatisticsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await reportRepository.GetRoomBookingStatisticsAsync(from, to, cancellationToken);
    }
}