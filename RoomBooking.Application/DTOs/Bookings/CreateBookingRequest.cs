namespace RoomBooking.Application.DTOs.Bookings;

using System.ComponentModel.DataAnnotations;

public class CreateBookingRequest
{
    [Required]
    public Guid RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public IReadOnlyList<Guid> ServiceIds { get; set; } = [];
}