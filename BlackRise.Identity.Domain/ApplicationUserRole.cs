using Microsoft.AspNetCore.Identity;

namespace BlackRise.Identity.Domain;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public virtual ApplicationUser User { get; set; }
    public virtual ApplicationRole Role { get; set; }
}
