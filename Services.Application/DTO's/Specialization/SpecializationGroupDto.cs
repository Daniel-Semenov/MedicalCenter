using Services.Application.DTO_s.Service;

namespace Services.Application.DTO_s.Specialization;

public class SpecializationGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<ServiceDto> Consultations { get; set; } = new();
}