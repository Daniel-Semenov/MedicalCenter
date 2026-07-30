using MediatR;
using Services.Application.DTO_s;
using Services.Application.DTO_s.Specialization;

namespace Services.Application.Queries.Services;

public class GetConsultationsQuery : IRequest<PagedResult<SpecializationGroupDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
