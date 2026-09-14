using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Queries.Handlers;

public class GetOfficeByIdQueryHandler : IRequestHandler<GetOfficeByIdQuery, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeResponseFactory _officeResponseFactory;

    public GetOfficeByIdQueryHandler(
        IOfficeRepository officeRepository,
        IOfficeResponseFactory officeResponseFactory)
    {
        _officeRepository = officeRepository;
        _officeResponseFactory = officeResponseFactory;
    }

    public async Task<OfficeResponse> Handle(GetOfficeByIdQuery request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (office is null)
        {
            throw new OfficeNotFoundException($"Office with id {request.Id} not found");
        }

        return await _officeResponseFactory.CreateAsync(office, cancellationToken);
    }
}