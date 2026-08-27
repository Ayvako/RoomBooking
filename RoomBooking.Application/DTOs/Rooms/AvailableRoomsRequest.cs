namespace RoomBooking.Application.DTOs.Rooms;

public class AvailableRoomsRequest
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Capacity { get; set; }
}