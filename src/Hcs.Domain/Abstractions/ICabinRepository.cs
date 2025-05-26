using Hcs.Domain.Cabins;

namespace Hcs.Domain.Abstractions;

public interface ICabinRepository
{
    Task<Cabin?> Get(CabinId id, CancellationToken ct);
}
