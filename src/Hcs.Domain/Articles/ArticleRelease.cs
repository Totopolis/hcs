using Domain.Shared;
using Hcs.Domain.Locales;

namespace Hcs.Domain.Articles;

public sealed class ArticleRelease : AggregateRoot<ArticleReleaseId>
{
    private ArticleRelease(ArticleReleaseId id) : base(id)
    {
    }

    public required Article Article { get; init; }

    public required Locale Locale { get; init; }

    public required ArticleContent Content { get; init; }

    public DateTimeOffset Published { get; init; }

    internal static ArticleRelease Create(
        Article article,
        Locale locale,
        ArticleContent content,
        DateTimeOffset now)
    {
        var id = ArticleReleaseId.From(Guid.CreateVersion7());
        return new ArticleRelease(id)
        {
            Article = article,
            Locale = locale,
            Content = content,
            Published = now
        };
    }
}
