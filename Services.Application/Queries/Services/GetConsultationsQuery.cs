using MediatR;
using Services.Application.DTO_s.Specialization;

namespace Services.Application.Queries.Services;

public class GetConsultationsQuery : IRequest<ConsultationsGroupedResponse>
{
}