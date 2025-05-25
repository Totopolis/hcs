using Application.Shared.Settings;

namespace Hcs.Server.Settings;

public class StartupSettings : ISettings
{
    public static string SectionName => "Startup";

    public required ScalarSettings Scalar { get; init; }

    public required CorsSettings Cors { get; init; }

    public class CorsSettings
    {
        public bool Enable { get; init; }

        public required IReadOnlyList<string> Origins { get; init; }
    }

    public class ScalarSettings
    {
        public bool Enable { get; init; }

        public string? Server { get; init; }
    }
}
