using Application.Shared.Settings;

namespace Hcs.Infrastructure.Settings;

public class InfrastructureSettings : ISettings
{
    public static string SectionName => "Infrastructure";

    public required string DatabaseConnectionString { get; init; }
}
