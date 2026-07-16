using Microsoft.AspNetCore.Identity;

namespace Authorization.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
        = new List<ApplicationUserRole>();
}