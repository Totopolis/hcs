using Application.Shared.Abstractions;
using Infrastructure.Shared.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Shared;

public static partial class ServiceExtensions
{
    public static IServiceCollection AddSharedInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddSingleton<ISystemInfo, SystemInfo.SystemInfo>();

        //services
        //    .AddScoped<IEventBusPublisher, EventBusPublisher>();

        return services;
    }
}
