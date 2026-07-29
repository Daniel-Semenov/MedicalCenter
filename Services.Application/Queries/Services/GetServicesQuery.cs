using MediatR;
using Services.Application.DTO_s.Service;

namespace Services.Application.Queries.Services;

public class GetServicesQuery : IRequest<List<ServiceDto>>
{
}