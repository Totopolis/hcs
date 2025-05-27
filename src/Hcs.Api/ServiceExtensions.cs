using ErrorOr;
using FastEndpoints;
using Hcs.Api.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Hcs.Api;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddFastEndpoints(opt =>
            {
                opt.Assemblies = [typeof(ServiceExtensions).Assembly];
            });

        return services;
    }

    public static IServiceCollection AddHcsAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Constants.AllAuthenticsPolicy,
                policy => policy.RequireRole("regular-role"));

            options.AddPolicy(
                Constants.KeeperAndRegularPolicy,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireRole("keeper-role"));
        });

        return services;
    }

    public static T ValueOrThrow<T>(
        this IErrorOr<T> errorOr,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (errorOr.IsError)
        {
            throw new ErrorOrException(errorOr, memberName, sourceLineNumber);
        }

        return errorOr.Value;
    }
}
