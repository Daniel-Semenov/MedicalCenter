using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.AppointmentResults;

public class CreateAppointmentResultCommandHandler : IRequestHandler<CreateAppointmentResultCommand, Result<Guid>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentResultRepository _appointmentResultRepository;

    public CreateAppointmentResultCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentResultRepository appointmentResultRepository)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentResultRepository = appointmentResultRepository;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        var appointmentResult = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (!appointmentResult.IsSuccess)
        {
            return Result<Guid>.Failure(AppointmentResultErrors.RelatedAppointmentNotFound(request.AppointmentId));
        }

        var existingResult = await _appointmentResultRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);

        if (existingResult.IsSuccess)
        {
            return Result<Guid>.Failure(AppointmentResultErrors.AlreadyExists(request.AppointmentId));
        }

        var appointmentResultEntity = new Domain.Entities.AppointmentResult
        {
            Id = Guid.NewGuid(),
            AppointmentId = request.AppointmentId,
            Complaints = request.Complaints,
            Conclusion = request.Conclusion,
            Recommendations = request.Recommendations,
            CreatedAt = DateTime.UtcNow
        };

        var addResult = await _appointmentResultRepository.AddAsync(appointmentResultEntity, cancellationToken);

        if (!addResult.IsSuccess)
        {
            return Result<Guid>.Failure(addResult.Error);
        }

        var appointment = appointmentResult.Value;
        appointment.Status = AppointmentStatus.Completed;
        appointment.UpdatedAt = DateTime.UtcNow;

        var updateAppointmentResult = await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        if (!updateAppointmentResult.IsSuccess)
        {
            return Result<Guid>.Failure(updateAppointmentResult.Error);
        }

        var saveResult = await _appointmentResultRepository.SaveChangesAsync(cancellationToken);

        return saveResult.IsSuccess
            ? Result<Guid>.Success(appointmentResultEntity.Id)
            : Result<Guid>.Failure(saveResult.Error);
    }
}