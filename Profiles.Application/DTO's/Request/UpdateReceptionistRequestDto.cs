namespace Profiles.Application.DTO_s.Request;

public class UpdateReceptionistRequestDto
{
    public Guid Id { get; set; }
    public Guid? OfficeId { get; set; }
}