using Application.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hcs.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddHcsApplicationOptions(
        this IServiceCollection services)
    {
        /*services.AddSettingsWithValidation<
            ApplicationSettings,
            ApplicationSettingsValidator,
            RegistryModule>();*/

        return services;
    }

    public static IServiceCollection AddHcsApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IModule, HcsModule>();

        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplicationSettings>());

        return services;
    }
}
