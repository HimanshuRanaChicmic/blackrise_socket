using Microsoft.AspNetCore.Identity;

namespace BlackRise.Identity.Domain;

public class ApplicationRole : IdentityRole<Guid>
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<ApplicationUserRole> Roles { get; set; }
    public ICollection<ApplicationRoleClaim> Claims { get; set; }
}