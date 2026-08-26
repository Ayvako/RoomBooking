namespace RoomBooking.Application.DTOs.Bookings;

public class UpdateBookingRequest
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}