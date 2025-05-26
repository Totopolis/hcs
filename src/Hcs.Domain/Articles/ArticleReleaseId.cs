using Vogen;

namespace Hcs.Domain.Articles;

[ValueObject<Guid>]
public partial struct ArticleReleaseId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("ArticleReleaseId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("ArticleReleaseId must be 7 version");
        }

        return Validation.Ok;
    }
}
