using Vogen;

namespace Hcs.Domain.Locales;

[ValueObject<Guid>]
public partial struct LocaleId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("LocaleId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("LocaleId must be 7 version");
        }

        return Validation.Ok;
    }
}
