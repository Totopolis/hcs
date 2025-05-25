using NodaMoney;
using NodaTime;
using System.Text.Json;

namespace Domain.Shared;

public static class Helpers
{
    private static Instant RoundToMicroseconds(this Instant instant)
    {
        const long TicksPerMicrosecond = NodaConstants.TicksPerMillisecond / 1000;
        long ticks = instant.ToUnixTimeTicks();

        long roundedTicks = (long)Math.Round((double)ticks / TicksPerMicrosecond) * TicksPerMicrosecond;

        return Instant.FromUnixTimeTicks(roundedTicks);
    }

    public static Instant GetInstantNow(this TimeProvider timeProvider)
    {
        var result = Instant.FromDateTimeUtc(timeProvider.GetUtcNow().UtcDateTime)
            .RoundToMicroseconds();

        return result;
    }

    public static readonly JsonDocument EmptyJsonDocument =
        JsonDocument.Parse("{}");

    public static readonly JsonElement EmptyJsonElement =
        EmptyJsonDocument.RootElement;

    public static readonly Money ZeroMoney = Money.FromDecimal(0.0m);

    public static LocalDate GetStartOfMonth(this Instant instant)
    {
        ZonedDateTime utcZoned = instant.InUtc();
        var firstDayOfMonth = new LocalDate(utcZoned.Year, utcZoned.Month, 1);

        // Получение начала дня в UTC и преобразование в Instant
        // return firstDayOfMonth.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant()
        return firstDayOfMonth;
    }
}
