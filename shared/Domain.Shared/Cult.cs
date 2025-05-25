using System.Globalization;

namespace Domain.Shared;

public sealed class Cult
{
    private readonly Dictionary<string, string> _dic = new();

    private Cult()
    {
    }

    public static Cult En(string str)
    {
        var cult = new Cult();
        cult._dic["en"] = str;
        return cult;
    }

    public Cult Ru(string str)
    {
        _dic["ru"] = str;
        return this;
    }

    public string Select()
    {
        var cult = CultureInfo.CurrentCulture.Name;
        if (cult == "ru-RU" && _dic.TryGetValue("ru", out var ruVal))
        {
            return ruVal;
        }

        return _dic["en"];
    }

    public override string ToString()
    {
        return Select();
    }

    public static implicit operator string(Cult wrapper)
    {
        return wrapper.Select();
    }
}
