using Appointments.Application.DTO_s.Response;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.AppointmentResults;

public class DownloadAppointmentResultQuery : IRequest<Result<FileDownloadDto>>
{
    public Guid Id { get; set; }
}