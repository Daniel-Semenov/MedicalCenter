using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class DeleteSpecializationCommandHandler : IRequestHandler<DeleteSpecializationCommand, Unit>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublisher _publisher;

    public DeleteSpecializationCommandHandler(
        ISpecializationRepository specializationRepository,
        IPublisher publisher)
    {
        _specializationRepository = specializationRepository;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.Id);
        }

        await _specializationRepository.DeleteAsync(specialization, cancellationToken);

        await _publisher.Publish(new SpecializationDeletedNotification(specialization.Id), cancellationToken);

        await _specializationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}