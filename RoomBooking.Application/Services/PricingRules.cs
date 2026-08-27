namespace RoomBooking.Application.Services;

public static class PricingRules
{
    public const decimal MorningDiscount = 0.90m;

    public const decimal EveningDiscount = 0.80m;

    public const decimal PeakSurcharge = 1.15m;

    public static decimal GetMultiplier(DateTime time)
    {
        var currentTime = time.TimeOfDay;

        if (currentTime >= new TimeSpan(12, 0, 0) &&
            currentTime < new TimeSpan(14, 0, 0))
        {
            return 1.15m;
        }

        if (currentTime >= new TimeSpan(6, 0, 0) &&
            currentTime < new TimeSpan(9, 0, 0))
        {
            return 0.9m;
        }

        if (currentTime >= new TimeSpan(18, 0, 0) &&
            currentTime < new TimeSpan(23, 0, 0))
        {
            return 0.8m;
        }

        return 1.0m;
    }
}