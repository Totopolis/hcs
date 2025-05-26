using Hcs.Domain.Locales;

namespace Hcs.Domain.Abstractions;

public interface ILocaleRepository
{
    Task<IReadOnlyList<Locale>> GetAll(CancellationToken ct);

    Task<Locale?> Find(string locale, CancellationToken ct);
}
