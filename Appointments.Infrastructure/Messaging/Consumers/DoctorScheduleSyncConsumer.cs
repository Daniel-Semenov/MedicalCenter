using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Messaging.Contracts.Events.Doctors;
using Microsoft.Extensions.Logging;

namespace Appointments.Infrastructure.Messaging.Consumers;

public class DoctorScheduleSyncConsumer : IConsumer<DoctorScheduleUpdated>
{
    private readonly ILogger<DoctorScheduleSyncConsumer> _logger;
    private readonly IDoctorScheduleSnapshotRepository _scheduleSnapshotRepository;

    public DoctorScheduleSyncConsumer(
        ILogger<DoctorScheduleSyncConsumer> logger,
        IDoctorScheduleSnapshotRepository scheduleSnapshotRepository)
    {
        _logger = logger;
        _scheduleSnapshotRepository = scheduleSnapshotRepository;
    }

    public async Task Consume(ConsumeContext<DoctorScheduleUpdated> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received DoctorScheduleUpdated event: DoctorId={DoctorId}", message.DoctorId);

        var snapshots = message.Entries.Select(e => new DoctorScheduleSnapshot
        {
            Id = Guid.NewGuid(),
            DoctorId = message.DoctorId,
            DayOfWeek = e.DayOfWeek,
            StartTime = e.StartTime,
            EndTime = e.EndTime
        });

        await _scheduleSnapshotRepository.ReplaceForDoctorAsync(message.DoctorId, snapshots, context.CancellationToken);

        _logger.LogInformation("DoctorScheduleSnapshot replaced for DoctorId: {DoctorId}", message.DoctorId);
    }
}