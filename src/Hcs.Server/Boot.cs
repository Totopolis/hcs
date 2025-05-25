using Application.Shared.Settings;
using FastEndpoints;
using Hcs.Api;
using Hcs.Api.Common;
using Hcs.Application;
using Hcs.Infrastructure;
using Hcs.Server.Settings;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using Server.Shared;

namespace Hcs.Server;

internal static class Boot
{
    public const string CorsPolicyName = "Hcs.Cors.Policy";

    public static void PreBuild(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables("G_");
        // builder.Configuration.AddJsonFile("appsettings.Production.json");

        var startupSettings = builder.Configuration.ValidateAndReturnPreBuildSettings<
            StartupSettings,
            StartupSettingsValidator,
            ServerModule>();

        // https://fast-endpoints.com/docs/configuration-settings#specify-json-serializer-options
        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.WriteIndented = true);

        builder.Services
            .AddOpenTelemetryLogsTo<ServerModule>(builder)
            .AddOpenTelemetryTracesOrMetricsTo<ServerModule>(
                builder,
                tracerProviderBuilder: builder =>
                {
                    // Masstransit
                    // builder.AddSource(DiagnosticHeaders.DefaultListenerName);
                    // builder.AddSource(RequestActivities.ActivitiesName);
                },
                meterProviderBuilder: builder =>
                {
                    // MassTransit
                    // builder.AddMeter(InstrumentationOptions.MeterName);
                });

        if (startupSettings.Scalar.Enable)
        {
            builder.Services.AddOpenApi();
        }

        if (startupSettings.Cors.Enable)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                {
                    policy
                        .WithOrigins(startupSettings.Cors.Origins.ToArray())
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    //.AllowAnyOrigin();
                });
            });
        }

        builder.Services
            .AddSingleton<TimeProvider>(x => TimeProvider.System);

        builder.Services
            .AddHcsApplicationOptions()
            .AddHcsInfrastructureOptions();

        builder.Services.AddSettingsWithValidation<
            StartupSettings,
            StartupSettingsValidator,
            ServerModule>();

        // Services
        builder.Services
            .AddHcsApplicationServices(builder.Configuration)
            .AddHcsInfrastructureServices(builder.Configuration);

        // Infrastructure shared services: system info & eventBusPublisher
        builder.Services
            .AddSharedInfrastructureServices(builder.Configuration);

        // Hosted services
        builder.Services
            .AddHcsInfrastructureHostedServices(builder.Configuration);

        // API
        builder.Services.AddApiServices(builder.Configuration);
    }

    public static Task PostBuild(this WebApplication app)
    {
        // TODO: use pipeline-class for each functions
        app.UseFastEndpoints()
           .UseHcsExceptionHandler();

        var startupSettings = app.Configuration.ValidateAndReturnPreBuildSettings<
            StartupSettings,
            StartupSettingsValidator,
            ServerModule>();

        if (startupSettings.Scalar.Enable)
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                if (string.IsNullOrWhiteSpace(startupSettings.Scalar.Server))
                {
                    options.Servers = [];
                }
                else
                {
                    options.Servers = [new ScalarServer(startupSettings.Scalar.Server)];
                }
            });
        }

        if (startupSettings.Cors.Enable)
        {
            app.UseCors(CorsPolicyName);
        }

        return Task.CompletedTask;
    }
}
