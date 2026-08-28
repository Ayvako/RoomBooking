namespace RoomBooking.Application.Services;

using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Entities;

/// <summary>
/// Calculates the total booking price based on room rate,
/// selected services, booking duration, and pricing rules.
/// </summary>
public class PricingService : IPricingService
{
    /// <summary>
    /// Calculates the total price for a booking.
    /// </summary>
    /// <param name="room">The room being booked.</param>
    /// <param name="services">The services selected for the booking.</param>
    /// <param name="startTime">The booking start time.</param>
    /// <param name="endTime">The booking end time.</param>
    /// <returns>The calculated total booking price.</returns>
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