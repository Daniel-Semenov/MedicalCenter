using MediatR;
using MassTransit;
using Messaging.Contracts.Events.Specializations;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class DeleteSpecializationCommandHandler : IRequestHandler<DeleteSpecializationCommand, Unit>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublishEndpoint _publishEndpoint; 

    public DeleteSpecializationCommandHandler(
        ISpecializationRepository specializationRepository,
        IPublishEndpoint publishEndpoint) 
    {
        _specializationRepository = specializationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.Id);
        }

        await _specializationRepository.DeleteAsync(specialization, cancellationToken);

        var integrationEvent = new SpecializationDeleted
        {
            Id = specialization.Id,
            DeletedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        await _specializationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}