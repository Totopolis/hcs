using Domain.Shared;
using ErrorOr;

namespace Hcs.Domain.CabinTokens;

public sealed class CabinToken : AggregateRoot<CabinTokenId>
{
    private CabinToken(CabinTokenId id) : base(id)
    {
    }

    public DateTimeOffset Created { get; init; }

    public string Value { get; init; } = default!;

    public DateTimeOffset Expired { get; init; }

    internal static ErrorOr<CabinToken> Create(
        DateTimeOffset expired,
        DateTimeOffset now)
    {
        if (now >= expired)
        {
            return Error.Validation("Cabin", "Invalid expired time");
        }

        var id = CabinTokenId.From(Guid.CreateVersion7());
        return new CabinToken(id)
        {
            Created = now,
            Value = Guid.NewGuid().ToString("N"),
            Expired = expired,
        };
    }
}
