using FluentValidation;

namespace Server.Shared.Consul;

internal sealed class ConsulSettingsValidator : AbstractValidator<ConsulSettings>
{
    public ConsulSettingsValidator()
    {
        RuleFor(x => x.EnvironmentName).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.ApplicationName).NotEmpty();
        RuleFor(x => x.Url).NotEmpty();

        RuleFor(x => x.Url)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.Url))
            .WithMessage($"{nameof(ConsulSettings.Url)} must be a valid URL");
    }
}
