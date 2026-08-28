namespace RoomBooking.Application.Services;

/// <summary>
/// Contains pricing multipliers used to calculate room booking costs.
/// </summary>
public static class PricingRules
{
    /// <summary>
    /// Discount multiplier applied during the morning period.
    /// </summary>
    public const decimal MorningDiscount = 0.90m;

    /// <summary>
    /// Discount multiplier applied during the evening period.
    /// </summary>
    public const decimal EveningDiscount = 0.80m;

    /// <summary>
    /// Surcharge multiplier applied during the peak period.
    /// </summary>
    public const decimal PeakSurcharge = 1.15m;

    /// <summary>
    /// Gets the pricing multiplier applicable to the specified time.
    /// </summary>
    /// <param name="time">The date and time for which the multiplier is calculated.</param>
    /// <returns>The pricing multiplier applicable to the specified time.</returns>
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