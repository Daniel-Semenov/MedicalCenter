using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Service;

namespace Services.Application.Queries.Services;

public class GetDiagnosticsQuery : IRequest<PagedResult<ServiceDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
