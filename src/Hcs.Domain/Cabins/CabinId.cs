using Vogen;

namespace Hcs.Domain.Cabins;

[ValueObject<Guid>]
public partial struct CabinId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("CabinId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("CabinId must be 7 version");
        }

        return Validation.Ok;
    }
}
