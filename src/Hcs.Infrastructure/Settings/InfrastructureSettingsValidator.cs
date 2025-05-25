using FluentValidation;

namespace Hcs.Infrastructure.Settings;

internal sealed class InfrastructureSettingsValidator : AbstractValidator<InfrastructureSettings>
{
    public InfrastructureSettingsValidator()
    {
        RuleFor(x => x.DatabaseConnectionString)
            .NotEmpty();
    }
}
