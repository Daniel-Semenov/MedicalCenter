using Appointments.Application.DTO_s.Response.TimeSlot;
using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.TimeSlots;

public class GetAvailableTimeSlotsQueryHandler
    : IRequestHandler<GetAvailableTimeSlotsQuery, Result<IReadOnlyList<TimeSlotDto>>>
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public GetAvailableTimeSlotsQueryHandler(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<Result<IReadOnlyList<TimeSlotDto>>> Handle(
        GetAvailableTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var slots = await _timeSlotRepository.GetAvailableAsync(
            request.DoctorId, request.FromDate, request.ToDate, cancellationToken);

        IReadOnlyList<TimeSlotDto> dtos = slots.Select(s => new TimeSlotDto
        {
            Id = s.Id,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();

        return Result<IReadOnlyList<TimeSlotDto>>.Success(dtos);
    }
}