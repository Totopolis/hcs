using Application.Shared.Settings;
using FastEndpoints;
using Hcs.Api;
using Hcs.Api.Common;
using Hcs.Application;
using Hcs.Infrastructure;
using Hcs.Server.Settings;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Server.Shared;
using System.Security.Claims;
using System.Text.Json;

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

        SetupAuth(builder, startupSettings);

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

    private static void SetupAuth(
        WebApplicationBuilder builder,
        StartupSettings settings)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = settings.Keycloak.Authority;
            // options.Audience = settings.Keycloak.Audience;
            options.RequireHttpsMetadata = settings.Keycloak.RequireHttpsMetadata;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                // Keycloak Client Scope Mapping if other clients used (api+insomna)
                // Or use both audiences
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.Keycloak.Authority,
                ValidAudiences = settings.Keycloak.Audiences,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // TODO: log it as warning
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Additional token validation
                    var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                    if (claimsIdentity != null)
                    {
                        // Extract from realm_access and add as standart role claims
                        var realmAccessClaim = claimsIdentity.FindFirst("realm_access")?.Value;
                        if (!string.IsNullOrEmpty(realmAccessClaim))
                        {
                            try
                            {
                                var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessClaim);
                                if (realmAccess.TryGetProperty("roles", out var roles))
                                {
                                    foreach (var role in roles.EnumerateArray())
                                    {
                                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                Console.WriteLine($"Error parsing realm_access: {ex.Message}");
                            }
                        }

                        // Extract from resource_access if need
                        var resourceAccessClaim = claimsIdentity.FindFirst("resource_access")?.Value;
                        if (!string.IsNullOrEmpty(resourceAccessClaim))
                        {
                            try
                            {
                                var resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessClaim);
                                var clientId = settings.Keycloak.ClientId;
                                if (resourceAccess.TryGetProperty(clientId!, out var clientAccess) &&
                                    clientAccess.TryGetProperty("roles", out var clientRoles))
                                {
                                    foreach (var role in clientRoles.EnumerateArray())
                                    {
                                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                Console.WriteLine($"Error parsing resource_access: {ex.Message}");
                            }
                        }

                        // Console.WriteLine($"User authenticated: {claimsIdentity.Name}");
                        // Console.WriteLine($"Roles: {string.Join(", ", claimsIdentity.FindAll(ClaimTypes.Role).Select(c => c.Value))}");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddHcsAuthorization(builder.Configuration);
    }

    public static Task PostBuild(this WebApplication app)
    {
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

        app.UseAuthentication();
        app.UseAuthorization();

        return Task.CompletedTask;
    }
}
