using BlackRise.Identity.Domain;
using BlackRise.Identity.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlackRise.Identity.Persistence.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        _ = builder.HasData
            (
                new ApplicationRole
                {
                    Id = Guid.Parse("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
                    Name = Role.SuperAdmin.ToString(),
                    NormalizedName = Role.SuperAdmin.ToString().ToUpper(),
                    ConcurrencyStamp = "ff3ef92a-9757-468c-a3ec-00a14209fb2c",
                    CreatedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("4a9d216c-4f7f-429d-9a28-a084526ce818"),
                    Name = Role.Admin.ToString(),
                    NormalizedName = Role.Admin.ToString().ToUpper(),
                    ConcurrencyStamp = "65b48457-7e70-4361-8cc0-7e7c46bc9dac",
                    CreatedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("920c0369-9b15-493d-b576-d806a271f748"),
                    Name = Role.User.ToString(),
                    NormalizedName = Role.User.ToString().ToUpper(),
                    ConcurrencyStamp = "02722f18-b994-445e-9cb4-37197481e878",
                    CreatedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = Guid.Parse("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                }
            );
    }
}
