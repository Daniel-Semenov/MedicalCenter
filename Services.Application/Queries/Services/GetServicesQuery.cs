using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Service;
using Services.Application.Interfaces;

namespace Services.Application.Queries.Services;

public class GetServicesQuery : IRequest<PagedResult<ServiceDto>>, IPagedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}