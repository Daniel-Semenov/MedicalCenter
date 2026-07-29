using MediatR;
using Services.Application.DTO_s.Service;

namespace Services.Application.Queries.Services;

public class GetServiceByIdQuery : IRequest<ServiceDto>
{
    public Guid Id { get; set; }
}