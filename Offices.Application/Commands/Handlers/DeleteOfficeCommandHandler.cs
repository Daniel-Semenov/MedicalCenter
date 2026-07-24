using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;

namespace Offices.Application.Commands.Handlers;

public class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand>
{
    private readonly IOfficeRepository _officeRepository;

    public DeleteOfficeCommandHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var exists = await _officeRepository.ExistsAsync(request.Id, cancellationToken);

        if (!exists)
        {
            throw new KeyNotFoundException($"Office with id {request.Id} not found");
        }
        
        await _officeRepository.DeleteAsync(request.Id, cancellationToken);
    }
}