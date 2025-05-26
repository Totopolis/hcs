using ErrorOr;

namespace Hcs.Application.Diagnostics;

public static class ApplicationErrors
{
    public static readonly Error LocaleNotFound = Error.NotFound(
        code: "Hcs.LocaleNotFound",
        description: "Locale not fount");

    public static readonly Error CabinNotFound = Error.NotFound(
        code: "Hcs.CabinNotFound",
        description: "Cabin not fount");

    public static readonly Error ArticleNotFound = Error.NotFound(
        code: "Hcs.ArticleNotFound",
        description: "Article not fount");
}
