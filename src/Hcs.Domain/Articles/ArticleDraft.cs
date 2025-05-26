using Domain.Shared;
using Hcs.Domain.Locales;

namespace Hcs.Domain.Articles;

public sealed class ArticleDraft : AggregateRoot<ArticleDraftId>
{
    private ArticleDraft(ArticleDraftId id) : base(id)
    {
    }

    public required Article Article { get; init; }

    public required Locale Locale { get; init; }

    public required ArticleContent Content { get; init; }

    public DateTimeOffset Created { get; init; }

    internal static ArticleDraft Create(
        Article article,
        Locale locale,
        ArticleContent content,
        DateTimeOffset now)
    {
        var id = ArticleDraftId.From(Guid.CreateVersion7());
        return new ArticleDraft(id)
        {
            Article = article,
            Locale = locale,
            Content = content,
            Created = now
        };
    }
}
