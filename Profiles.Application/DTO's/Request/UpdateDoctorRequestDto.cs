namespace Profiles.Application.DTO_s.Request;

public class UpdateDoctorRequestDto
{
    public Guid Id { get; set; }
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
    public int CareerStartYear { get; set; }
}