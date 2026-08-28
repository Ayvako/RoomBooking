namespace RoomBooking.Application.DTOs.RoomServices;

using System.ComponentModel.DataAnnotations;

public class UpdateRoomServiceRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal Price { get; set; }
}