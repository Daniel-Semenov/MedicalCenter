using Appointments.Domain.Entities;
using Appointments.Shared.Common.Pagination;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<Result<Appointment>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Appointment>>> GetByPatientIdAsync(Guid patientId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Appointment>>> GetByDoctorIdAsync(Guid doctorId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsSlotTakenAsync(Guid doctorId, DateTime appointmentDate, DateTime startTime, Guid? excludeAppointmentId = null, CancellationToken cancellationToken = default);
    Task<Result<Appointment>> AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}