namespace RoomBooking.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.DTOs.Reports;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Enums;
using RoomBooking.Infrastructure.Persistence;

public class ReportRepository(RoomBookingDbContext context) : IReportRepository
{
    public async Task<BookingStatisticsResponse> GetBookingStatisticsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var bookings = await context.Bookings
            .Where(booking =>
                booking.StartTime >= from &&
                booking.StartTime < to)
            .ToListAsync(cancellationToken);

        var activeBookings = bookings
            .Where(booking => booking.Status == BookingStatus.Active)
            .ToList();

        var cancelledBookings = bookings
            .Count(booking => booking.Status == BookingStatus.Cancelled);

        var totalRevenue = activeBookings.Sum(booking => booking.TotalPrice);

        var averageBookingPrice = activeBookings.Count == 0
            ? 0
            : activeBookings.Average(booking => booking.TotalPrice);

        var totalBookedHours = activeBookings.Sum(booking => (decimal)(booking.EndTime - booking.StartTime).TotalHours);

        return new BookingStatisticsResponse
        {
            TotalBookings = bookings.Count,
            ActiveBookings = activeBookings.Count,
            CancelledBookings = cancelledBookings,
            TotalRevenue = totalRevenue,
            AverageBookingPrice = averageBookingPrice,
            TotalBookedHours = totalBookedHours,
        };
    }
}