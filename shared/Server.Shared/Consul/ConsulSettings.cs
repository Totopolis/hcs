using Application.Shared.Settings;

namespace Server.Shared.Consul;

public sealed class ConsulSettings : ISettings
{
    public static string SectionName => "Consul";

    public required bool Enable { get; init; }

    public required string Url { get; init; }

    public required string Token { get; init; }

    public required string ApplicationName { get; init; }

    public required string EnvironmentName { get; init; }
}
