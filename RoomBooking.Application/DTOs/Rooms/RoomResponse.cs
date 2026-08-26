namespace RoomBooking.Application.DTOs.Rooms;

using RoomBooking.Application.DTOs.RoomServices;

public class RoomResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal BaseHourlyRate { get; set; }

    public IReadOnlyList<RoomServiceResponse> Services { get; set; } = [];
}