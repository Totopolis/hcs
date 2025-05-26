using Domain.Shared;
using ErrorOr;
using Hcs.Domain.Cabins;
using Hcs.Domain.Diagnostics;
using Hcs.Domain.Locales;

namespace Hcs.Domain.Articles;

public sealed class Article : AggregateRoot<ArticleId>
{
    public const string DefaultTitle = "No Content";

    private readonly List<ArticleDraft> _drafts = new();
    private readonly List<ArticleRelease> _release = new();

    private Article(ArticleId id) : base(id)
    {
    }

    public required Cabin Cabin { get; init; }

    public required AggregateCode Code { get; init; }

    public required Locale OriginalLocale { get; init; }

    public bool MultiLangSupport { get; init; }

    public bool DraftSupport { get; init; }

    public DateTimeOffset Created { get; init; }

    public string Title { get; private set; } = DefaultTitle;

    public IReadOnlyList<ArticleDraft> Drafts => _drafts.AsReadOnly();

    public IReadOnlyList<ArticleRelease> Releases => _release.AsReadOnly();

    // TODO: comments support
    public static Article Create(
        Cabin cabin,
        AggregateCode code,
        Locale originalLocale,
        bool multiLangSupport,
        bool draftSupport,
        DateTimeOffset now)
    {
        var id = ArticleId.From(Guid.CreateVersion7());
        return new Article(id)
        {
            Cabin = cabin,
            Code = code,
            OriginalLocale = originalLocale,
            MultiLangSupport = multiLangSupport,
            DraftSupport = draftSupport,
            Created = now,
            Title = DefaultTitle
        };
    }

    public ErrorOr<Success> UpsertContent(
        Locale locale,
        ArticleContent content,
        DateTimeOffset now)
    {
        if (!MultiLangSupport && locale != OriginalLocale)
        {
            return DomainErrors.BadLocale;
        }

        if (OriginalLocale == locale)
        {
            Title = content.ShortTitle;
        }

        if (DraftSupport)
        {
            var draft = ArticleDraft.Create(
                article: this,
                locale: locale,
                content: content,
                now: now);

            _drafts.RemoveAll(x => x.Locale == locale);
            _drafts.Add(draft);

            return Result.Success;
        }

        var release = ArticleRelease.Create(
            article: this,
            locale: locale,
            content: content,
            now: now);

        _release.RemoveAll(x => x.Locale == locale);
        _release.Add(release);

        return Result.Success;
    }

    // After publish all drafts removed
    public void PublishAll(DateTimeOffset now)
    {
        if (!DraftSupport)
        {
            return;
        }

        var draftsToRelease = _drafts
            .Select(x => ArticleRelease.Create(
                article: this,
                locale: x.Locale,
                content: x.Content,
                now: now))
            .ToList();

        var stayReleased = _release
            .Where(x => !draftsToRelease.Any(y => y.Locale == x.Locale))
            .ToList();

        _release.Clear();
        _drafts.Clear();

        _release.AddRange(draftsToRelease);
        _release.AddRange(stayReleased);
    }
}
