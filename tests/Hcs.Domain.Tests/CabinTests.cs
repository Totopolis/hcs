using Hcs.Domain.Cabins;
using Hcs.Domain.Locales;

namespace Hcs.Domain.Tests;

public class CabinTests
{
    [Fact]
    public void SuccefullCreate()
    {
        var locale = Locale.Create("ru", "ru");

        var cabin = Cabin.Create(
            originalLocale: locale);

        Assert.Equal(locale, cabin.OriginalLocale);
    }

    [Fact]
    public void SuccessCreateToken()
    {
        var locale = Locale.Create("ru", "ru");
        var cabin = Cabin.Create(
            originalLocale: locale);

        var errorOrToken = cabin.CreateToken(
            expired: DateTimeOffset.Now + TimeSpan.FromSeconds(100),
            now: DateTimeOffset.Now);

        Assert.False(errorOrToken.IsError);
        Assert.Single(cabin.Tokens);
    }
}
