using Appointments.Application.Interfaces;
using Appointments.Application.Options;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Constants;
using Appointments.Shared.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Appointments.Application.Commands.TimeSlots;

public class GenerateTimeSlotsCommandHandler : IRequestHandler<GenerateTimeSlotsCommand, Result>
{
    private readonly IDoctorSnapshotQuery _doctorSnapshotQuery;
    private readonly IDoctorScheduleSnapshotRepository _scheduleSnapshotRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly TimeSlotGenerationOptions _options;
    private readonly ILogger<GenerateTimeSlotsCommandHandler> _logger;

    public GenerateTimeSlotsCommandHandler(
        IDoctorSnapshotQuery doctorSnapshotQuery,
        IDoctorScheduleSnapshotRepository scheduleSnapshotRepository,
        ITimeSlotRepository timeSlotRepository,
        IOptions<TimeSlotGenerationOptions> options, ILogger<GenerateTimeSlotsCommandHandler> logger)
    {
        _doctorSnapshotQuery = doctorSnapshotQuery;
        _scheduleSnapshotRepository = scheduleSnapshotRepository;
        _timeSlotRepository = timeSlotRepository;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<Result> Handle(GenerateTimeSlotsCommand request, CancellationToken cancellationToken)
    {
        var doctors = await _doctorSnapshotQuery.GetActiveAsync(cancellationToken);

        var today = DateTime.UtcNow.Date;
        var windowEnd = today.AddDays(_options.WindowDays);

        foreach (var doctor in doctors)
        {
            if (doctor.OfficeId is null)
            {
                _logger.LogWarning("Doctor {DoctorId} has no OfficeId assigned. Skipping slot generation for this doctor.", doctor.Id);
                continue;
            }

            var scheduleEntries = await _scheduleSnapshotRepository.GetByDoctorIdAsync(doctor.Id, cancellationToken);

            if (scheduleEntries.Count == 0)
            {
                _logger.LogWarning("Doctor {DoctorId} has no schedule. Skipping.", doctor.Id);
                continue;
            }

            var existingStarts = await _timeSlotRepository.GetExistingStartTimesAsync(
                doctor.Id, today, windowEnd, cancellationToken);

            var newSlots = new List<TimeSlot>();

            for (var day = today; day < windowEnd; day = day.AddDays(1))
            {
                var entriesForDay = scheduleEntries.Where(e => e.DayOfWeek == day.DayOfWeek);

                foreach (var entry in entriesForDay)
                {
                    var slotStart = day.Add(entry.StartTime);
                    var dayEnd = day.Add(entry.EndTime);

                    while (slotStart.AddMinutes(AppointmentConstants.SlotDurationMinutes) <= dayEnd)
                    {
                        if (!existingStarts.Contains(slotStart))
                        {
                            newSlots.Add(new TimeSlot
                            {
                                Id = Guid.NewGuid(),
                                DoctorId = doctor.Id,
                                OfficeId = doctor.OfficeId.Value,
                                StartTime = slotStart,
                                EndTime = slotStart.AddMinutes(AppointmentConstants.SlotDurationMinutes),
                                Status = TimeSlotStatus.Available
                            });
                        }

                        slotStart = slotStart.AddMinutes(AppointmentConstants.SlotDurationMinutes);
                    }
                }
            }

            if (newSlots.Count > 0)
            {
                await _timeSlotRepository.AddRangeAsync(newSlots, cancellationToken);
            }
            else
            {
                 _logger.LogInformation(
                    "No new slots to generate for doctor {DoctorId} ({DoctorName}). " +
                    "All slots in the window already exist or schedule is empty.",
                    doctor.Id,
                    doctor.FirstName + " " + doctor.LastName ?? "Unnamed");
            }
        }

        return await _timeSlotRepository.SaveChangesAsync(cancellationToken);
    }
}