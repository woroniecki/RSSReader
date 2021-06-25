using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataLayer.Code.Configs
{
    class IdentityUserConfig : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> entity)
        {
            entity.Property(m => m.Id).HasMaxLength(85);
            entity.Property(m => m.NormalizedEmail).HasMaxLength(85);
            entity.Property(m => m.NormalizedUserName).HasMaxLength(85);
        }
    }

    class IdentityRoleConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> entity)
        {
            entity.Property(m => m.Id).HasMaxLength(85);
            entity.Property(m => m.NormalizedName).HasMaxLength(85);
        }
    }

    class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> entity)
        {
            entity.Property(m => m.LoginProvider).HasMaxLength(85);
            entity.Property(m => m.ProviderKey).HasMaxLength(85);
            entity.Property(m => m.UserId).HasMaxLength(85);
        }
    }

    class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> entity)
        {
            entity.Property(m => m.UserId).HasMaxLength(85);
            entity.Property(m => m.RoleId).HasMaxLength(85);
        }
    }

    class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> entity)
        {
            entity.Property(m => m.UserId).HasMaxLength(85);
            entity.Property(m => m.LoginProvider).HasMaxLength(85);
            entity.Property(m => m.Name).HasMaxLength(85);
        }
    }

    class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> entity)
        {
            entity.Property(m => m.Id).HasMaxLength(85);
            entity.Property(m => m.UserId).HasMaxLength(85);
        }
    }

    class IdentityRoleClaimConfig : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> entity)
        {
            entity.Property(m => m.Id).HasMaxLength(85);
            entity.Property(m => m.RoleId).HasMaxLength(85);
        }
    }
}
