using Application.Shared.Abstractions;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using static Hcs.Api.RootEndpoint;

namespace Hcs.Api;

// https://fast-endpoints.com/docs/get-started#union-type-returning-handler
public sealed class RootEndpoint : EndpointWithoutRequest<RootResponse>
{
    private readonly ILogger<RootEndpoint> _logger;
    private readonly ISystemInfo _systemInfo;

    // TODO: remove ISystemInfo reference, use mediatr instead
    public RootEndpoint(ILogger<RootEndpoint> logger, ISystemInfo systemInfo)
    {
        _logger = logger;
        _systemInfo = systemInfo;
    }

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogWarning("Root endpoint handler executing");

        await SendAsync(new(
            Promt: "Hello funs!",
            Status: "Work",
            Application: _systemInfo.ApplicationName,
            Environment: _systemInfo.EnvironmentName,
            StartDateTime: _systemInfo.StartDateTime,
            Uptime: _systemInfo.Uptime,
            BuildDateTime: _systemInfo.BuildDateTime,
            Version: _systemInfo.Version));
    }

    public record RootResponse(
        string Promt,
        string Status,
        string Application,
        string Environment,
        DateTime StartDateTime,
        TimeSpan Uptime,
        DateTime BuildDateTime,
        string Version);
}
