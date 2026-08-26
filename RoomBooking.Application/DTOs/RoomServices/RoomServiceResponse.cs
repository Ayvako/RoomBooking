namespace RoomBooking.Application.DTOs.RoomServices;

public class RoomServiceResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}