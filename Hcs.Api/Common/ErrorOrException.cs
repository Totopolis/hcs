using ErrorOr;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Hcs.Api.Common;

public sealed class ErrorOrException : Exception
{
    private readonly IErrorOr _error;
    private readonly string _callerMemberName;
    private readonly int _callerLineNumber;

    public ErrorOrException(
        IErrorOr error,
        string callerMemberName,
        int callerLineNumber)
    {
        _error = error;
        _callerMemberName = callerMemberName;
        _callerLineNumber = callerLineNumber;
    }

    public string CallerMemberName => _callerMemberName;

    public int CallerLineNumber => _callerLineNumber;

    public bool IsError => _error.IsError;

    public IReadOnlyList<Error> Errors => _error.Errors ?? [];

    // https://www.milanjovanovic.tech/blog/problem-details-for-aspnetcore-apis
    public ProblemDetails ConvertToProblem(
        string instance)
    {
        if (Errors.Any(e => e.Type == ErrorType.Unauthorized))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Unauthorized",
                Errors = Errors.Select(x => new ProblemDetails.Error
                {
                    Code = x.Code,
                    Name = x.Description
                })
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.Forbidden))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status403Forbidden,
                Detail = "Forbidden",
                Errors = Errors.Select(x => new ProblemDetails.Error
                {
                    Code = x.Code,
                    Name = x.Description
                })
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.Conflict))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status409Conflict,
                Detail = "Conflict error",
                Errors = Errors.Select(x => new ProblemDetails.Error
                {
                    Code = x.Code,
                    Name = x.Description
                })
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.NotFound))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status404NotFound,
                Detail = "Not found error",
                Errors = Errors.Select(x => new ProblemDetails.Error
                {
                    Code = x.Code,
                    Name = x.Description
                })
            };
        }

        if (Errors.Any(e => e.Type == ErrorType.Validation))
        {
            return new ProblemDetails
            {
                Instance = instance,
                Status = StatusCodes.Status400BadRequest,
                Detail = "Validation error",
                Errors = Errors.Select(x => new ProblemDetails.Error
                {
                    Code = x.Code,
                    Name = x.Description
                })
            };
        }

        return new ProblemDetails
        {
            Instance = instance,
            Status = StatusCodes.Status500InternalServerError,
            Detail = "Internal server error",
            Errors = Errors.Select(x => new ProblemDetails.Error
            {
                Code = x.Code,
                Name = x.Description
            })
        };
    }
}
