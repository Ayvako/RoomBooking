namespace RoomBooking.Application.DTOs.RoomServices;

public class CreateRoomServiceRequest
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}