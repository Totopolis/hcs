using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Hcs.Api.Common;

public static class ExceptionHandlerExtensions
{
    /// <summary>
    /// Registers the default global exception handler which will log the exceptions on the server and return a user-friendly json response to the client
    /// when unhandled exceptions occur.
    /// TIP: when using this exception handler, you may want to turn off the asp.net core exception middleware logging to avoid duplication like so:
    /// <code>
    /// "Logging": { "LogLevel": { "Default": "Warning", "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware": "None" } }
    /// </code>
    /// </summary>
    /// <param name="logStructuredException">set to true if you'd like to log the error in a structured manner</param>
    /// <param name="useGenericReason">set to true if you don't want to expose the actual exception reason in the json response sent to the client</param>
    public static IApplicationBuilder UseHcsExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errApp => errApp
            .Run(async ctx => await ExceptionsHandler(ctx)));

        return app;
    }

    private static async Task ExceptionsHandler(HttpContext ctx)
    {
        var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();
        if (exHandlerFeature is null)
        {
            return;
        }

        ctx.Response.ContentType = "application/problem+json";
        var cancellationToken = ctx.RequestAborted;

        var problemInstance = exHandlerFeature.Endpoint?.DisplayName?.Split(" => ")[0] ?? string.Empty;
        var logger = ctx.Resolve<ILogger<RootEndpoint>>();

        if (exHandlerFeature.Error is ErrorOrException errorOr)
        {
            var problem = errorOr.ConvertToProblem(problemInstance);
            ctx.Response.StatusCode = problem.Status;

            var errors = string.Join(";", errorOr.Errors.Select(x => x.Code));
            var problemPlace = $"caller: {errorOr.CallerMemberName}, line: {errorOr.CallerLineNumber}";

            logger.LogError(
                exception: exHandlerFeature.Error,
                message: "[Application errors {@Errors}] at [{@ProblemInstance}] on [{@ProblemPlace}]",
                args: [errors, problemInstance, problemPlace]);

            await ctx.Response.WriteAsJsonAsync(problem, cancellationToken);
        }
        else
        {
            var exceptionType = exHandlerFeature.Error.GetType().Name;
            var problemReason = exHandlerFeature.Error.Message;
            var stackTrace = exHandlerFeature.Error.StackTrace;

            logger.LogError(
                exception: exHandlerFeature.Error,
                message: "[Unhandled exception {@ExceptionType}] at [{@ProblemInstance}] due to [{@ProblemReason}]. StackTrace: [{@StackTrace}]",
                args: [exceptionType, problemInstance, problemReason, stackTrace]);

            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // TODO: if env is dev or staging - trace errors to clients
            var problem = new ProblemDetails
            {
                Instance = problemInstance,
                Status = StatusCodes.Status500InternalServerError,
                Detail = "One or more errors occurred"
            };

            await ctx.Response.WriteAsJsonAsync(problem, cancellationToken);
        }
    }
}
