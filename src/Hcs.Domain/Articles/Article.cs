using Domain.Shared;

namespace Hcs.Domain.Articles;

public sealed class Article : AggregateRoot<ArticleId>
{
    private Article(ArticleId id) : base(id)
    {
    }

    public static Article Create()
    {
        var id = ArticleId.From(Guid.CreateVersion7());
        return new Article(id);
    }
}
