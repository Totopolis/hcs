using Domain.Shared;

namespace Hcs.Domain.Locales;

public sealed class Locale : AggregateRoot<LocaleId>
{
    private Locale(LocaleId id) : base(id)
    {
    }

    public required string Name { get; init; }

    public required string Slug { get; init; }

    public static Locale Create(
        string name,
        string slug)
    {
        var id = LocaleId.From(Guid.CreateVersion7());
        return new Locale(id)
        {
            Name = name,
            Slug = slug
        };
    }
}
