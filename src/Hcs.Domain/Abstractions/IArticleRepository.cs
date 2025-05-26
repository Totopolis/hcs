using Hcs.Domain.Articles;
using Hcs.Domain.Cabins;

namespace Hcs.Domain.Abstractions;

public interface IArticleRepository
{
    Task<Article?> Get(ArticleId id, CancellationToken ct);

    Task<IEnumerable<Article>> GetAllWithoutContent(CabinId cabinId, CancellationToken ct);

    Task<IEnumerable<Article>> GetAllReleases(CabinId id, CancellationToken ct);

    Task<Article?> GetReleasedBySlug(
        CabinId cabinId,
        string slug,
        CancellationToken ct);

    Task<bool> CheckSlugExists(
        CabinId cabinId,
        string slug,
        CancellationToken ct);
}
