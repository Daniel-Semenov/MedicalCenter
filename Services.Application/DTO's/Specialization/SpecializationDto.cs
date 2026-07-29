using Services.Application.DTO_s.Service;
using Services.Domain.Enums;

namespace Services.Application.DTO_s.Specialization;

public class SpecializationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public SpecializationStatus Status { get; set; }
    public List<ServiceDto> Services { get; set; } = new();
}