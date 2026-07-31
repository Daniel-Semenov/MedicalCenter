namespace Profiles.Application.DTO_s.Request;

public class CreateReceptionistRequestDto
{
    public Guid AccountId { get; set; }
    public Guid? OfficeId { get; set; }
}