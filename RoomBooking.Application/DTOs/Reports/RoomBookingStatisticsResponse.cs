namespace RoomBooking.Application.DTOs.Reports;

public class RoomBookingStatisticsResponse
{
    public Guid RoomId { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public int BookingCount { get; set; }

    public decimal BookedHours { get; set; }

    public decimal Revenue { get; set; }
}