using FluentValidation;

namespace Server.Shared.OpenTelemetry;

internal class OpenTelemetrySettingsValidator : AbstractValidator<OpenTelemetrySettings>
{
    public OpenTelemetrySettingsValidator()
    {
        RuleFor(x => x.BaseUrl)
            .NotEmpty();

        RuleFor(x => x.BaseUrl)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.BaseUrl))
            .WithMessage($"{nameof(OpenTelemetrySettings.BaseUrl)} must be a valid URL");

        RuleFor(x => x.ServiceName)
            .NotEmpty();
    }
}
