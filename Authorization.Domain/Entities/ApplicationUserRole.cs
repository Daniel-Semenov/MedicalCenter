using Microsoft.AspNetCore.Identity;

namespace Authorization.Domain.Entities;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public Account User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}