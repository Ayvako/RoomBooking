namespace RoomBooking.Application.Interfaces;

using RoomBooking.Domain.Entities;

public interface IPricingService
{
    decimal Calculate(Room room, IEnumerable<RoomService> services, DateTime startTime, DateTime endTime);
}