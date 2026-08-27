namespace RoomBooking.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal BaseHourlyRate { get; set; }

    public List<RoomService> Services { get; set; } = [];

    public List<Booking> Bookings { get; set; } = [];
}