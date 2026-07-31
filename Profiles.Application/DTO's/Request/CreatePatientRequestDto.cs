namespace Profiles.Application.DTO_s.Request;

public class CreatePatientRequestDto
{
    public Guid AccountId { get; set; }
    public bool IsLinkedToAccount { get; set; } = true;
}