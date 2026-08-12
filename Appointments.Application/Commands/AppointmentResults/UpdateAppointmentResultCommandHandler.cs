using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.AppointmentResults;

public class UpdateAppointmentResultCommandHandler : IRequestHandler<UpdateAppointmentResultCommand, Result>
{
    private readonly IAppointmentResultRepository _appointmentResultRepository;

    public UpdateAppointmentResultCommandHandler(IAppointmentResultRepository appointmentResultRepository)
    {
        _appointmentResultRepository = appointmentResultRepository;
    }

    public async Task<Result> Handle(UpdateAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        var getResult = await _appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result.Failure(getResult.Error);
        }

        var appointmentResult = getResult.Value;
        appointmentResult.Complaints = request.Complaints;
        appointmentResult.Conclusion = request.Conclusion;
        appointmentResult.Recommendations = request.Recommendations;
        appointmentResult.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _appointmentResultRepository.UpdateAsync(appointmentResult, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result.Failure(updateResult.Error);
        }

        return await _appointmentResultRepository.SaveChangesAsync(cancellationToken);
    }
}