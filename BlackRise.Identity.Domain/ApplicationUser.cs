using Microsoft.AspNetCore.Identity;

namespace BlackRise.Identity.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? EmailConfirmationCode { get; set; }
    public DateTime? EmailConfirmationCodeExpiry { get; set; }

    public string? ResetPasswordCode { get; set; }
    public DateTime? ResetPasswordCodeExpiry { get; set; }
    public DateTime? LastResetCodeConfirmTime { get; set; }
    public DateTime? LastEmailCodeConfirmTime { get; set; }
    public bool IsResetCodeConfirmed { get; set; }

    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public string? AppleId { get; set; }
    public bool IsProfileCreated { get; set; }
    public bool IsProfileCompleted { get; set; }
    public bool IsSocialLogin { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}
