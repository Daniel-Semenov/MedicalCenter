namespace Profiles.Application.DTO_s.Response;

public class PatientResponseDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public bool IsLinkedToAccount { get; set; }
}
