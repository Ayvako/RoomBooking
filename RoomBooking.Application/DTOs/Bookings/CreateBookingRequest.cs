namespace RoomBooking.Application.DTOs.Bookings;

public class CreateBookingRequest
{
    public Guid RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public IReadOnlyList<Guid> ServiceIds { get; set; } = [];
}