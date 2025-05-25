namespace Application.Shared.Abstractions;

public interface ISystemInfo
{
    // TODO: may be, use NodaTime?
    DateTime BuildDateTime { get; }

    DateTime StartDateTime { get; }

    TimeSpan Uptime { get; }

    bool IsDevelopment { get; }

    string ApplicationName { get; }

    string EnvironmentName { get; }

    string Version { get; }
}
