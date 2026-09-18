using Authorization.Shared.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Authorization.Application.Behaviours;

public abstract class ValidationBehaviorBase<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    protected ValidationBehaviorBase(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    protected async Task<List<FluentValidation.Results.ValidationFailure>> GetFailures(
        TRequest request,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return new List<FluentValidation.Results.ValidationFailure>();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        return results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();
    }

    protected Error CreateError(List<FluentValidation.Results.ValidationFailure> failures)
    {
        var message = string.Join("; ", failures.Select(f => f.ErrorMessage));
        return new Error("VALIDATION_ERROR", message, StatusCodes.Status400BadRequest);
    }
}