using Vogen;

namespace Hcs.Domain.CabinTokens;

[ValueObject<Guid>]
public partial struct CabinTokenId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("CabinTokenId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("CabinTokenId must be 7 version");
        }

        return Validation.Ok;
    }
}
