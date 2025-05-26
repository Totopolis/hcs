using Hcs.Domain.Articles;
using Hcs.Domain.Cabins;
using Hcs.Domain.Locales;

namespace Hcs.Domain.Tests;

public class ArticleTests
{
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    private readonly Locale _locale = Locale.Create("ru", "ru");

    private readonly Cabin _cabin = Cabin.Create(
        originalLocale: Locale.Create("ru", "ru"));

    [Fact]
    public void SuccessCreate()
    {
        var article = Article.Create(
            cabin: _cabin,
            code: AggregateCode.From("abcd1"),
            originalLocale: _locale,
            multiLangSupport: true,
            draftSupport: true,
            now: _now);

        Assert.Equal(_locale, article.OriginalLocale);
        Assert.True(article.MultiLangSupport);
        Assert.True(article.DraftSupport);
        Assert.Equal(_now, article.Created);
    }

    [Fact]
    public void SuccessCreateContent()
    {
        var errorOrContent = ArticleContent.Create(
            slug: "what-is-it",
            title: "What is it",
            shortTitle: "Wtf",
            description: "Short description",
            toc: "Table of content",
            content: "Long read",
            tags: Array.Empty<string>());

        Assert.False(errorOrContent.IsError);
        Assert.Equal("what-is-it", errorOrContent.Value.Slug);
    }

    [Fact]
    public void SuccessContentValueObjectsCompare()
    {
        var errorOrContent1 = ArticleContent.Create(
            slug: "what-is-it",
            title: "What is it",
            shortTitle: "Wtf",
            description: "Short description",
            toc: "Table of content",
            content: "Long read",
            tags: Array.Empty<string>());

        var errorOrContent2 = ArticleContent.Create(
            slug: "what-is-it",
            title: "What is it",
            shortTitle: "Wtf",
            description: "Short description",
            toc: "Table of content",
            content: "Long read",
            tags: Array.Empty<string>());

        Assert.Equal(errorOrContent1.Value, errorOrContent2.Value);
    }
}
