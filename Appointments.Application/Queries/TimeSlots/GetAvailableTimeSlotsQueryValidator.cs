using FluentValidation;

namespace Appointments.Application.Queries.TimeSlots;

public class GetAvailableTimeSlotsQueryValidator : AbstractValidator<GetAvailableTimeSlotsQuery>
{
    public GetAvailableTimeSlotsQueryValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x).Must(x => x.ToDate > x.FromDate).WithMessage("ToDate must be after FromDate");
    }
}