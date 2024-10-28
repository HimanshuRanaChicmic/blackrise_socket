using BlackRise.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlackRise.Identity.Persistence.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.HasData(new ApplicationUserRole
        {
            RoleId = Guid.Parse("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
            UserId = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461")
        });

        builder.HasData(new ApplicationUserRole
        {
            RoleId = Guid.Parse("4a9d216c-4f7f-429d-9a28-a084526ce818"),
            UserId = Guid.Parse("23e09a08-ebde-4c5d-94f9-6a222c1a6362")
        });

        builder.HasData(new ApplicationUserRole
        {
            RoleId = Guid.Parse("920c0369-9b15-493d-b576-d806a271f748"),
            UserId = Guid.Parse("3e96af2d-c6f2-4f9e-843e-ada493a79a17")
        });
    }
}
