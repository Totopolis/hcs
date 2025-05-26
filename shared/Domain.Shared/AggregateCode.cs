using Vogen;

namespace Hcs.Domain.Articles;

[ValueObject<string>]
public partial struct AggregateCode
{
    private static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Validation.Invalid("AggregateCode can not be empty");
        }

        if (value.Length < 5)
        {
            return Validation.Invalid("AggregateCode length must be greater than 4");
        }

        if (!Char.IsLetter(value[0]))
        {
            return Validation.Invalid("First char must be letter");
        }

        if (!value.All(x => Char.IsLetterOrDigit(x)))
        {
            return Validation.Invalid("All chars must be letter or digit");
        }

        return Validation.Ok;
    }
}
