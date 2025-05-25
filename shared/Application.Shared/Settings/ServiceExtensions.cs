using Application.Shared.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.Shared.Settings;

public static class ServiceExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder)
    where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(provider =>
            new FluentValidationOptions<TOptions>(
                // TODO: specify argument name
                name: optionsBuilder.Name,
                scopeFactory: provider.GetRequiredService<IServiceScopeFactory>()));

        return optionsBuilder;
    }

    public static OptionsBuilder<TOptions> AddSettingsWithValidation<TOptions, TValidator, TModule>(
        this IServiceCollection services)
    where TOptions : class, ISettings
    where TValidator : class, IValidator<TOptions>
    where TModule : IModule
    {
        // Add the validator
        services.AddScoped<IValidator<TOptions>, TValidator>();

        var path = TModule.Name + ":" + TOptions.SectionName;

        return services.AddOptions<TOptions>()
            .BindConfiguration(path)
            .ValidateFluentValidation()
            .ValidateOnStart();
    }

    public static TOptions ValidateAndReturnPreBuildSettings<TOptions, TValidator, TModule>(
        this IConfiguration configuration)
    where TOptions : class, ISettings
    where TValidator : class, IValidator<TOptions>, new()
    where TModule : IModule
    {
        var path = TModule.Name + ":" + TOptions.SectionName;
        var section = configuration.GetRequiredSection(path);
        var settings = section.Get<TOptions>();

        if (settings is null)
        {
            throw new SectionNotFoundException(path);
        }

        var validator = new TValidator();
        validator.ValidateAndThrow(settings);

        return settings;
    }

}
