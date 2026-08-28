namespace RoomBooking.Application.DTOs.Rooms;

using System.ComponentModel.DataAnnotations;

public class CreateRoomRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int Capacity { get; set; }

    [Range(typeof(decimal), "0.01", "1000000000")]
    public decimal BaseHourlyRate { get; set; }
}