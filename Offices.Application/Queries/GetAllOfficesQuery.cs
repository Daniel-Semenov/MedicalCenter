using MediatR;
using Offices.Application.DTO_s;

namespace Offices.Application.Queries;

public class GetAllOfficesQuery : IRequest<IEnumerable<OfficeResponse>>
{
}