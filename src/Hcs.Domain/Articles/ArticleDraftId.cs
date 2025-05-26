using Vogen;

namespace Hcs.Domain.Articles;

[ValueObject<Guid>]
public partial struct ArticleDraftId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("ArticleDraftId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("ArticleDraftId must be 7 version");
        }

        return Validation.Ok;
    }
}
