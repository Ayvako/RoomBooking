namespace RoomBooking.Domain.Entities;

using RoomBooking.Domain.Enums;

public class Booking
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Room Room { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; }

    public List<RoomService> Services { get; set; } = [];
}