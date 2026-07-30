using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Service;

namespace Services.Application.Queries.Services;

public class GetServicesQuery : IRequest<PagedResult<ServiceDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
