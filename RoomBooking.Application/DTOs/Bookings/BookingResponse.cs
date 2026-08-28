namespace RoomBooking.Application.DTOs.Bookings;

using RoomBooking.Application.DTOs.Rooms;
using RoomBooking.Domain.Enums;

public class BookingResponse
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; }

    public RoomResponse Room { get; set; } = null!;
}