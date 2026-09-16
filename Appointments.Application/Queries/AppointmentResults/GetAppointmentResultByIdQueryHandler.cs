using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.AppointmentResults;

public class GetAppointmentResultByIdQueryHandler
    : IRequestHandler<GetAppointmentResultByIdQuery, Result<AppointmentResultDto>>
{
    private readonly IAppointmentResultRepository _appointmentResultRepository;
    private readonly IAppointmentResultDtoBuilder _dtoBuilder;

    public GetAppointmentResultByIdQueryHandler(
        IAppointmentResultRepository appointmentResultRepository,
        IAppointmentResultDtoBuilder dtoBuilder)
    {
        _appointmentResultRepository = appointmentResultRepository;
        _dtoBuilder = dtoBuilder;
    }

    public async Task<Result<AppointmentResultDto>> Handle(
        GetAppointmentResultByIdQuery request,
        CancellationToken cancellationToken)
    {
        var resultEntityResult = await _appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!resultEntityResult.IsSuccess)
        {
            return Result<AppointmentResultDto>.Failure(resultEntityResult.Error);
        }

        return await _dtoBuilder.BuildAsync(resultEntityResult.Value, cancellationToken);
    }
}