using Services.Domain.Enums;

namespace Services.Application.DTO_s.Specialization;

public class UpdateSpecializationRequest
{
    public string Name { get; set; } = null!;
    public SpecializationStatus Status { get; set; }
}