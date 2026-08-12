using Appointments.Shared.Common.Results;
using FluentValidation;
using MediatR;

namespace Appointments.Application.Behaviours;

public class ResultValidationBehavior<TRequest, TValue> 
    : ValidationBehaviorBase<TRequest, Result<TValue>>,
        IPipelineBehavior<TRequest, Result<TValue>>
    where TRequest : IRequest<Result<TValue>>
{
    public ResultValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        : base(validators) { }

    public async Task<Result<TValue>> Handle(
        TRequest request,
        RequestHandlerDelegate<Result<TValue>> next,
        CancellationToken cancellationToken)
    {
        var failures = await GetFailures(request, cancellationToken);
        if (failures.Any())
        {
            return Result<TValue>.Failure(CreateError(failures));
        }

        return await next();
    }
}