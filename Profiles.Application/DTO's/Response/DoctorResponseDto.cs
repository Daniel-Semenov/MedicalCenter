namespace Profiles.Application.DTO_s.Response;

public class DoctorResponseDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
    public int CareerStartYear { get; set; }
}

