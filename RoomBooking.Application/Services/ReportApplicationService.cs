namespace RoomBooking.Application.Services;

using RoomBooking.Application.DTOs.Reports;
using RoomBooking.Application.Interfaces;

public class ReportApplicationService(IReportRepository reportRepository)
{
    public async Task<BookingStatisticsResponse> GetBookingStatisticsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await reportRepository.GetBookingStatisticsAsync(from, to, cancellationToken);
    }
}