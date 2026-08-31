using MassTransit;
using Messaging.Contracts.Events.Doctors;
using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Application.Interfaces.Services;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;

namespace Profiles.Application.Services;

public class DoctorScheduleService : IDoctorScheduleService
{
    private readonly IDoctorScheduleRepository _scheduleRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public DoctorScheduleService(IDoctorScheduleRepository scheduleRepository, IPublishEndpoint publishEndpoint)
    {
        _scheduleRepository = scheduleRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<DoctorScheduleResponseDto>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var result = await _scheduleRepository.GetByDoctorIdAsync(doctorId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<DoctorScheduleResponseDto>.Failure(result.Error);
        }

        return Result<DoctorScheduleResponseDto>.Success(MapToDto(doctorId, result.Value));
    }

    public async Task<Result<DoctorScheduleResponseDto>> UpdateAsync(
        Guid doctorId, UpdateDoctorScheduleRequestDto request, CancellationToken cancellationToken = default)
    {
        var entries = request.Entries.Select(e => new DoctorSchedule
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            DayOfWeek = e.DayOfWeek,
            StartTime = e.StartTime,
            EndTime = e.EndTime
        }).ToList();

        var replaceResult = await _scheduleRepository.ReplaceForDoctorAsync(doctorId, entries, cancellationToken);

        if (!replaceResult.IsSuccess)
        {
            return Result<DoctorScheduleResponseDto>.Failure(replaceResult.Error);
        }

        var integrationEvent = new DoctorScheduleUpdated
        {
            DoctorId = doctorId,
            Entries = request.Entries.Select(e => new DoctorScheduleEntry
            {
                DayOfWeek = e.DayOfWeek,
                StartTime = e.StartTime,
                EndTime = e.EndTime
            }).ToList()
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        var saveResult = await _scheduleRepository.SaveChangesAsync(cancellationToken);

        if (!saveResult.IsSuccess)
        {
            return Result<DoctorScheduleResponseDto>.Failure(saveResult.Error);
        }

        return Result<DoctorScheduleResponseDto>.Success(MapToDto(doctorId, entries));
    }

    private static DoctorScheduleResponseDto MapToDto(Guid doctorId, IReadOnlyList<DoctorSchedule> entries) => new()
    {
        DoctorId = doctorId,
        Entries = entries.Select(e => new DoctorScheduleEntryDto
        {
            DayOfWeek = e.DayOfWeek,
            StartTime = e.StartTime,
            EndTime = e.EndTime
        }).ToList()
    };
}