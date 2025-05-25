using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.Shared.Settings;

internal sealed class FluentValidationOptions<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly string? _name;
    private readonly IServiceScopeFactory _scopeFactory;

    public FluentValidationOptions(
        string? name,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _name = name;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        // Null name is used to configure all named options.
        if (_name != null && _name != name)
        {
            // Ignored if not validating this instance.
            return ValidateOptionsResult.Skip;
        }

        // Ensure options are provided to validate against
        ArgumentNullException.ThrowIfNull(options);

        // Validators are registered as scoped, so need to create a scope,
        // as we will be called from the root scope
        using var scope = _scopeFactory.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();
        var results = validator.Validate(options);
        if (results.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        string typeName = options.GetType().Name;
        var errors = new List<string>();
        foreach (var result in results.Errors)
        {
            errors.Add($"Fluent validation failed for '{typeName}.{result.PropertyName}' with the error: '{result.ErrorMessage}'.");
        }

        return ValidateOptionsResult.Fail(errors);
    }
}
