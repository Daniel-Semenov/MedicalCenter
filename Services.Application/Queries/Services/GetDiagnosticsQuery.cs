using MediatR;
using Services.Application.DTO_s.Service;

namespace Services.Application.Queries.Services;

public class GetDiagnosticsQuery : IRequest<List<ServiceDto>>
{
}