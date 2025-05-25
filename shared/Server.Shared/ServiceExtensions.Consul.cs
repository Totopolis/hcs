using Application.Shared;
using Application.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Server.Shared.Consul;
using Winton.Extensions.Configuration.Consul;

namespace Server.Shared;

public static partial class ServiceExtensions
{
    public static void MergeServerSettingsFromConsulIfNeedIn<TModule>(
        this IConfigurationManager configurationManager,
        IConfiguration configuration)
    where TModule : class, IModule
    {
        var settings = configuration.ValidateAndReturnPreBuildSettings<
            ConsulSettings,
            ConsulSettingsValidator,
            TModule>();

        if (!settings.Enable)
        {
            return;
        }

        configurationManager
            .AddConsul(
                $"{settings.ApplicationName}-{settings.EnvironmentName}",
                opt =>
                {
                    opt.ConsulConfigurationOptions = cco =>
                    {
                        cco.Address = new Uri(settings.Url);
                        cco.Token = settings.Token;
                    };
                })
            .Build();
    }
}
