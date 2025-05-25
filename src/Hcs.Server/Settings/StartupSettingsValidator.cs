using FluentValidation;

namespace Hcs.Server.Settings;

internal sealed class StartupSettingsValidator : AbstractValidator<StartupSettings>
{
    public StartupSettingsValidator()
    {
    }
}
