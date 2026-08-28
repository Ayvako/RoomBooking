namespace RoomBooking.Application.DTOs.Reports;

public class BookingStatisticsResponse
{
    public int TotalBookings { get; set; }

    public int ActiveBookings { get; set; }

    public int CancelledBookings { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal AverageBookingPrice { get; set; }

    public decimal TotalBookedHours { get; set; }
}