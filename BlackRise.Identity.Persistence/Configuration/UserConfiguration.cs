using BlackRise.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlackRise.Identity.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        ApplicationUser superAdmin = new()
        {
            Id = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
            Email = "super-admin@blackrise.com",
            NormalizedEmail = "SUPER-ADMIN@BLACKRISE.COM",
            UserName = "super-admin@blackrise.com",
            NormalizedUserName = "SUPER-ADMIN@BLACKRISE.COM",
            ConcurrencyStamp = "231d6055-699c-4e30-acdd-7e270ae493ac",
            SecurityStamp = "b43e9e94-17a5-4f93-bb9f-2ebbf412187a",
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            CreatedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };
        superAdmin.PasswordHash = GeneratePasswordHash(superAdmin);


        ApplicationUser admin = new()
        {
            Id = Guid.Parse("23e09a08-ebde-4c5d-94f9-6a222c1a6362"),
            Email = "admin@blackrise.com",
            NormalizedEmail = "ADMIN@BLACKRISE.COM",
            UserName = "admin@blackrise.com",
            NormalizedUserName = "ADMIN@BLACKRISE.COM",
            ConcurrencyStamp = "231d6055-699c-4e30-acdd-7e270ae493ac",
            SecurityStamp = "b43e9e94-17a5-4f93-bb9f-2ebbf412187a",
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            CreatedBy = Guid.Parse("a901293b-f4cd-4b50-93da-158bc435c1f9"),
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = Guid.Parse("8494a3ad-b74b-4407-a9ff-7a3f0c17770d"),
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };
        admin.PasswordHash = GeneratePasswordHash(admin);

        ApplicationUser user = new()
        {
            Id = Guid.Parse("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
            Email = "user@blackrise.com",
            NormalizedEmail = "USER@BLACKRISE.COM",
            UserName = "user@blackrise.com",
            NormalizedUserName = "USER@BLACKRISE.COM",
            ConcurrencyStamp = "8c349594-2eb1-4b0b-9ff5-b8d473013cb9",
            SecurityStamp = "b98e9771-fb3c-4182-91d9-d7d3ebfd8959",
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            CreatedBy = Guid.Parse("a901293b-f4cd-4b50-93da-158bc435c1f9"),
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = Guid.Parse("8494a3ad-b74b-4407-a9ff-7a3f0c17770d"),
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };
        user.PasswordHash = GeneratePasswordHash(user);

        builder.Property(x => x.IsActive).HasDefaultValue(true);

        _ = builder.HasData(superAdmin, admin, user);
    }

    private string GeneratePasswordHash(ApplicationUser applicationUser)
    {
        PasswordHasher<ApplicationUser> passwordHasher = new();
        return passwordHasher.HashPassword(applicationUser, "Secure123!!");
    }
}
