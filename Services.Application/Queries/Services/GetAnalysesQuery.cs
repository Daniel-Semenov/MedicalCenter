using MediatR;
using Services.Application.DTO_s.Service;

namespace Services.Application.Queries.Services;

public class GetAnalysesQuery : IRequest<List<ServiceDto>>
{
}