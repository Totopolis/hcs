using Domain.Shared;
using ErrorOr;
using Hcs.Domain.CabinTokens;
using Hcs.Domain.Locales;

namespace Hcs.Domain.Cabins;

public sealed class Cabin : AggregateRoot<CabinId>
{
    private readonly List<CabinToken> _tokens = new();

    private Cabin(CabinId id) : base(id)
    {
    }

    public required Locale OriginalLocale { get; init; }

    public IReadOnlyList<CabinToken> Tokens => _tokens;

    public static Cabin Create(
        Locale originalLocale)
    {
        var id = CabinId.From(Guid.CreateVersion7());
        return new Cabin(id)
        {
            OriginalLocale = originalLocale,
        };
    }

    public ErrorOr<CabinToken> CreateToken(
        DateTimeOffset expired,
        DateTimeOffset now)
    {
        var errorOrToken = CabinToken.Create(expired, now);
        if (errorOrToken.IsError)
        {
            return errorOrToken.Errors;
        }

        _tokens.Add(errorOrToken.Value);

        return errorOrToken.Value;
    }
}
