using MediatR;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;

namespace Offices.Application.Queries.Handlers;

public class GetOfficeByIdQueryHandler : IRequestHandler<GetOfficeByIdQuery, OfficeResponse>
{
    private readonly IOfficeRepository _officeRepository;

    public GetOfficeByIdQueryHandler(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<OfficeResponse> Handle(GetOfficeByIdQuery request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (office is null)
        {
            throw new KeyNotFoundException($"Office with id {request.Id} not found");  
        }

        return new OfficeResponse
        {
            Id = office.Id,
            FullAddress = $"{office.City}, {office.Street} {office.HouseNumber}/{office.OfficeNumber}",
            IsActive = office.IsActive,
            PhoneNumber = office.PhoneNumber,
            PhotoUrl = office.PhotoUrl
        };
    }
}