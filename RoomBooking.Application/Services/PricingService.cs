namespace RoomBooking.Application.Services;

using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

public class PricingService : IPricingService
{
    public decimal Calculate(Room room, IEnumerable<RoomService> services, DateTime startTime, DateTime endTime)
    {
        var roomPrice = CalculateRoomPrice(room.BaseHourlyRate, startTime, endTime);

        var servicesPrice = services.Sum(service => service.Price);

        return roomPrice + servicesPrice;
    }

    private static decimal CalculateRoomPrice(decimal hourlyRate, DateTime startTime, DateTime endTime)
    {
        decimal total = 0;

        var current = startTime;

        while (current < endTime)
        {
            var nextHour = new DateTime(current.Year, current.Month, current.Day, current.Hour, 0, 0, DateTimeKind.Local).AddHours(1);

            if (nextHour > endTime)
            {
                nextHour = endTime;
            }

            var duration = (decimal)(nextHour - current).TotalHours;

            var multiplier = PricingRules.GetMultiplier(current);

            total += hourlyRate * duration * multiplier;

            current = nextHour;
        }

        return total;
    }
}