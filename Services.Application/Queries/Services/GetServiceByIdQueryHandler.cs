using MediatR;
using Services.Application.DTO_s.Service;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Queries.Services;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceByIdQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDto> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
            throw new NotFoundException(nameof(Service), request.Id);

        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            Category = service.Category,
            Status = service.Status,
            SpecializationId = service.SpecializationId
        };
    }
}