using Microsoft.AspNetCore.Identity;
using Profiles.DAL.Enums;

namespace Profiles.DAL.Entities;

public class Account : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Guid? PhotoId { get; set; } 
    public string? AvatarUrl { get; set; } 
    public Role Role { get; set; } 
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }  
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public Receptionist? Receptionist { get; set; }
}




