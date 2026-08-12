using Appointments.Domain.Entities;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Interfaces;

public interface IAppointmentResultRepository
{
    Task<Result<AppointmentResult>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AppointmentResult>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<Result<AppointmentResult>> AddAsync(AppointmentResult result, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(AppointmentResult result, CancellationToken cancellationToken = default);
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}