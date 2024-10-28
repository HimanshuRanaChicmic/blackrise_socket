using BlackRise.Identity.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlackRise.Identity.Persistence;

public class IdentityDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid,
    ApplicationUserClaim, ApplicationUserRole,ApplicationUserLogin,
    ApplicationRoleClaim,ApplicationUserToken>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : base(options) 
    {
        Database.SetCommandTimeout(180);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        _ = builder.Entity<ApplicationUser>().ToTable("Users");
        _ = builder.Entity<ApplicationRole>().ToTable("Roles");
        _ = builder.Entity<ApplicationUserRole>().ToTable("UserRoles");
        _ = builder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
        _ = builder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");
        _ = builder.Entity<ApplicationUserToken>().ToTable("UserTokens");
        _ = builder.Entity<ApplicationUserLogin>().ToTable("UserLogins");

        _ = builder.Entity<ApplicationUserRole>(userRole =>
        {
            userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

            userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.Roles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

            userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
        });

        _ = builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
