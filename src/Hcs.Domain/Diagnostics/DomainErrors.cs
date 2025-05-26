using ErrorOr;

namespace Hcs.Domain.Diagnostics;

public static class DomainErrors
{
    public static readonly Error BadLocale = Error.Validation(
        code: "Hcs.BadLocale",
        description: "Bad locale");

    public static class Article
    {
        public static readonly Error BadString = Error.Validation(
            code: "Hcs.Article.BadString",
            description: "Bad provided string");
    }
}
