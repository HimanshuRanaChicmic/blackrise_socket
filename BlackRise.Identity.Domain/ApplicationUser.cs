using Microsoft.AspNetCore.Identity;

namespace BlackRise.Identity.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? EmailConfirmationCode { get; set; }
    public DateTime? EmailConfirmationCodeExpiry { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}
