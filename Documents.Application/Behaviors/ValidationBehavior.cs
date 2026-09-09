using FluentValidation;
using MediatR;
using Documents.Shared.Common.Errors;
using Documents.Shared.Common.Results;
using Microsoft.AspNetCore.Http;

namespace Documents.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var error = new Error(
                "VALIDATION_FAILED",
                string.Join("; ", failures.Select(f => f.ErrorMessage)),
                StatusCodes.Status400BadRequest);

            var resultType = typeof(TResponse);
            
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultValueType = resultType.GetGenericArguments()[0];
                var failureMethod = typeof(Result<>)
                    .MakeGenericType(resultValueType)
                    .GetMethod(nameof(Result<object>.Failure), new[] { typeof(Error) });
                
                return (TResponse)failureMethod?.Invoke(null, new object[] { error })!;
            }
        }

        return await next();
    }
}