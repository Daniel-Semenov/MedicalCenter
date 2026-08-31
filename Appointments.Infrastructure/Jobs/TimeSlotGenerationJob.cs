using Appointments.Application.Commands.TimeSlots;
using MediatR;
using Quartz;

namespace Appointments.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class TimeSlotGenerationJob : IJob
{
    private readonly IMediator _mediator;

    public TimeSlotGenerationJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _mediator.Send(new GenerateTimeSlotsCommand(), context.CancellationToken);
    }
}