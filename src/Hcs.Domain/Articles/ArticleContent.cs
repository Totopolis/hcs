using CSharpFunctionalExtensions;
using ErrorOr;
using Hcs.Domain.Diagnostics;

namespace Hcs.Domain.Articles;

public sealed class ArticleContent : ValueObject
{
    private ArticleContent()
    {
    }

    public required string Slug { get; init; }

    public required string Title { get; init; }

    public required string ShortTitle { get; init; }

    public required string Description { get; init; }

    /// <summary>
    /// Table of content
    /// </summary>
    public required string Toc { get; init; }

    public required string Content { get; init; }

    public required IReadOnlyList<string> Tags { get; init; }

    public static ErrorOr<ArticleContent> Create(
        string slug,
        string title,
        string shortTitle,
        string description,
        string toc,
        string content,
        IReadOnlyList<string> tags)
    {
        if (string.IsNullOrWhiteSpace(slug) ||
            string.IsNullOrWhiteSpace(title) ||
            string.IsNullOrWhiteSpace(shortTitle) ||
            string.IsNullOrWhiteSpace(description) ||
            string.IsNullOrWhiteSpace(toc) ||
            string.IsNullOrWhiteSpace(content) ||
            tags.Any(string.IsNullOrWhiteSpace))
        {
            return DomainErrors.Article.BadString;
        }

        return new ArticleContent
        {
            Slug = slug,
            Title = title,
            ShortTitle = shortTitle,
            Description = description,
            Toc = toc,
            Content = content,
            Tags = tags.Order().ToList(),
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Slug;
        yield return Title;
        yield return ShortTitle;
        yield return Description;
        yield return Toc;
        yield return Content;
        yield return Tags.Aggregate((l, r) => l + ";" + r);
    }
}
