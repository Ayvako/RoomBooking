namespace RoomBooking.Application.Services;

using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

public class PricingService : IPricingService
{
    public decimal Calculate(Room room, IEnumerable<RoomService> services, DateTime startTime, DateTime endTime)
    {
        var hours = (decimal)(endTime - startTime).TotalHours;

        var roomPrice = room.BaseHourlyRate * hours;

        var servicesPrice = services.Sum(service => service.Price);

        return roomPrice + servicesPrice;
    }
}