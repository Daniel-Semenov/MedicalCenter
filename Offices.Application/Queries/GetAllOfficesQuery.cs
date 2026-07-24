using MediatR;
using Offices.Application.DTO_s;

namespace Offices.Application.Queries;

public class GetAllOfficesQuery : IRequest<PagedResult<OfficeResponse>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}