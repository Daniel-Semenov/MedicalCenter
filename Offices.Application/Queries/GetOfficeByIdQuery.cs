using MediatR;
using Offices.Application.DTO_s;

namespace Offices.Application.Queries;

public class GetOfficeByIdQuery : IRequest<OfficeResponse>
{
    public Guid Id { get; set; }
}