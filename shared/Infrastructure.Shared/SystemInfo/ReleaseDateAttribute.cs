using System.Globalization;
using System.Reflection;

namespace Infrastructure.Shared.SystemInfo;

/// <summary>
/// The release date assembly attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class ReleaseDateAttribute : Attribute
{
    /// <summary> 
    /// Constructor that takes in a DateTime string.
    /// </summary>
    /// <param name="utcDateString"> A DateTime 'O' (round-trip date/time) format string.</param>
    public ReleaseDateAttribute(string utcDateString)
    {
        ReleaseDate = DateTime.ParseExact(
            s: utcDateString,
            format: "O",
            provider: CultureInfo.InvariantCulture,
            style: DateTimeStyles.RoundtripKind);
    }

    /// <summary>
    /// The date the assembly was built
    /// </summary>
    public DateTime ReleaseDate { get; }

    /// <summary>
    /// Method to obtain the release date from the assembly attribute.
    /// </summary>
    /// <param name="assembly">An assembly instance. If not provided then will be used entry assembly.</param>
    /// <returns>The date time from the assembly attribute.</returns>
    public static DateTime GetReleaseDate(Assembly? assembly = null)
    {
        object[]? attribute = (assembly ?? Assembly.GetEntryAssembly())?
            .GetCustomAttributes(typeof(ReleaseDateAttribute), false);
        
        return attribute?.Length >= 1 ?
            ((ReleaseDateAttribute)attribute[0]).ReleaseDate :
            DateTime.MinValue;
    }
}
