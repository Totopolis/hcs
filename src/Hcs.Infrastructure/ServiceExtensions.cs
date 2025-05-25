using Application.Shared.Settings;
using Hcs.Application;
using Hcs.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hcs.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddHcsInfrastructureOptions(
        this IServiceCollection services)
    {
        services.AddSettingsWithValidation<
            InfrastructureSettings,
            InfrastructureSettingsValidator,
            HcsModule>();

        return services;
    }

    public static IServiceCollection AddHcsInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // It is scoped service
        // services.AddDbContext<RegistryDbContext>();

        // services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddHcsInfrastructureHostedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddHostedService<PrepareHostedService>();

        return services;
    }
}
