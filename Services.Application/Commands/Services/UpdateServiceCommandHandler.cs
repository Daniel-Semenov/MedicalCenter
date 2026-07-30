using MediatR;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Unit>
{
    private readonly IServiceRepository _serviceRepository;

    public UpdateServiceCommandHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Unit> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }

        service.Name = request.Name;
        service.Price = request.Price;
        service.Status = request.Status;
        service.UpdatedAt = DateTime.UtcNow;

        await _serviceRepository.UpdateAsync(service, cancellationToken);

        return Unit.Value;
    }
}