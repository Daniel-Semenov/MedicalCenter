namespace Profiles.Application.DTO_s.Request;

public class UpdatePatientRequestDto
{
    public Guid Id { get; set; }
    public bool IsLinkedToAccount { get; set; }
}