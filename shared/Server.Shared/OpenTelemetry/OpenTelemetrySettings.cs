using Application.Shared.Settings;

namespace Server.Shared.OpenTelemetry;

public record OpenTelemetrySettings : ISettings
{
    public static string SectionName => "OpenTelemetry";

    public bool SuppressConsole { get; init; }

    public bool EnableLogs { get; init; }

    public bool EnableMetrics { get; init; }

    public bool EnableTraces { get; init; }

    public required string BaseUrl { get; init; }

    public required IReadOnlyDictionary<string, string> Headers { get; init; }

    public required string ServiceName { get; init; }
}
