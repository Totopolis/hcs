using Hcs.Domain.Articles;
using Hcs.Domain.Cabins;
using Hcs.Domain.Locales;

namespace Hcs.Domain.Tests;

public class ArticleContentTests
{
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    private readonly Locale _locale = Locale.Create("ru", "ru");

    private readonly Cabin _cabin = Cabin.Create(
        originalLocale: Locale.Create("ru", "ru"));

    private readonly AggregateCode _code = AggregateCode.From("abcd1");

    private readonly ArticleContent _content = ArticleContent.Create(
        slug: "what-is-it",
        title: "What is it",
        shortTitle: "Wtf",
        description: "Short description",
        toc: "Table of content",
        content: "Long read",
        tags: Array.Empty<string>()).Value;

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void CreateArticleItem(
        bool multiLangSupport,
        bool draftSupport)
    {
        var article = Article.Create(
            cabin: _cabin,
            code: _code,
            originalLocale: _locale,
            multiLangSupport: multiLangSupport,
            draftSupport: draftSupport,
            now: _now);

        var errorOrSuccess = article.UpsertContent(
            locale: _locale,
            content: _content,
            now: _now);

        Assert.False(errorOrSuccess.IsError);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DenyLocaleInSingleLanguage(bool draftSupport)
    {
        var article = Article.Create(
            cabin: _cabin,
            code: _code,
            originalLocale: _locale,
            multiLangSupport: false,
            draftSupport: draftSupport,
            now: _now);

        var errorOrSuccess = article.UpsertContent(
            locale: Locale.Create("kor", "kor"),
            content: _content,
            now: _now);

        Assert.True(errorOrSuccess.IsError);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Publish(
        bool multiLangSupport,
        bool draftSupport)
    {
        var article = Article.Create(
            cabin: _cabin,
            code: _code,
            originalLocale: _locale,
            multiLangSupport: multiLangSupport,
            draftSupport: draftSupport,
            now: _now);

        _ = article.UpsertContent(
            locale: _locale,
            content: _content,
            now: _now);

        article.PublishAll(_now);

        Assert.Empty(article.Drafts);
        Assert.Single(article.Releases);
    }
}
