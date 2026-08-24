using MediatR;
using MassTransit;
using Messaging.Contracts.Events.Specializations;
using Messaging.Contracts.Events.Enums;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class UpdateSpecializationCommandHandler : IRequestHandler<UpdateSpecializationCommand, Unit>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateSpecializationCommandHandler(
        ISpecializationRepository specializationRepository,
        IPublishEndpoint publishEndpoint) 
    {
        _specializationRepository = specializationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.Id);
        }
        
        var exists = await _specializationRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken);

        if (exists)
        {
            throw new SpecializationAlreadyExistsException(request.Name);
        }

        specialization.Name = request.Name;
        specialization.Status = request.Status;
        specialization.UpdatedAt = DateTime.UtcNow;

        await _specializationRepository.UpdateAsync(specialization, cancellationToken);

        var integrationEvent = new SpecializationUpdated
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Status = (SpecializationStatusSnapshot)specialization.Status,
            UpdatedAt = specialization.UpdatedAt.Value
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        await _specializationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}