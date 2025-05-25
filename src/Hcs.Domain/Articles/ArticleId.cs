using Vogen;

namespace Hcs.Domain.Articles;

[ValueObject<Guid>]
public partial struct ArticleId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("ArticleId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("ArticleId must be 7 version");
        }

        return Validation.Ok;
    }
}
