using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Code.Configs
{
    public class GroupConfig : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> entity)
        {
            entity.HasIndex(p => p.Guid)
                  .IsUnique();

            entity.Property(x => x.Guid).HasDefaultValueSql("NEWID()");
        }
    }
}
