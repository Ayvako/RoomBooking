namespace RoomBooking.Application.Interfaces
{
    using RoomBooking.Application.DTOs.Reports;

    public interface IReportRepository
    {
        Task<BookingStatisticsResponse> GetBookingStatisticsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}