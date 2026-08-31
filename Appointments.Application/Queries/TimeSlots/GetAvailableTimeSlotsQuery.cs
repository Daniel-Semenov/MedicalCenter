using Appointments.Application.DTO_s.Response.TimeSlot;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.TimeSlots;

public class GetAvailableTimeSlotsQuery : IRequest<Result<IReadOnlyList<TimeSlotDto>>>
{
    public Guid DoctorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}