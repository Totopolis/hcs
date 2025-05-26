using ErrorOr;

namespace Hcs.Application.Diagnostics;

public static class ApplicationErrors
{
    public static readonly Error CabinNotFound = Error.NotFound(
        code: "Hcs.CabinNotFound",
        description: "Cabin not fount");
}
