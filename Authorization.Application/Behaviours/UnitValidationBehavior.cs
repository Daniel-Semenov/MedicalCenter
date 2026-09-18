using Authorization.Shared.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Authorization.Application.Behaviours;

public class UnitValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public UnitValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var message = string.Join("; ", failures.Select(f => f.ErrorMessage));
        var error = new Error("VALIDATION_ERROR", message, StatusCodes.Status400BadRequest);

        var result = Result.Failure(error);
        return (TResponse)(object)result;
    }
}