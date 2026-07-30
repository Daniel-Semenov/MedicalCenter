using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Services.Application.Interfaces;

namespace Services.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IEnumerable<IValidator<IPagedQuery>> _pagedQueryValidators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        IEnumerable<IValidator<IPagedQuery>> pagedQueryValidators)
    {
        _validators = validators;
        _pagedQueryValidators = pagedQueryValidators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();

        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults =
                await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            failures.AddRange(validationResults.SelectMany(r => r.Errors).Where(f => f != null));
        }

        if (request is IPagedQuery pagedQuery && _pagedQueryValidators.Any())
        {
            var context = new ValidationContext<IPagedQuery>(pagedQuery);
            var validationResults =
                await Task.WhenAll(_pagedQueryValidators.Select(v => v.ValidateAsync(context, cancellationToken)));
            failures.AddRange(validationResults.SelectMany(r => r.Errors).Where(f => f != null));
        }

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
