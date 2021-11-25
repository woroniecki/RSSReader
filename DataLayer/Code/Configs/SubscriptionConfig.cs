using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Code.Configs
{
    public class SubscriptionConfig : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> entity)
        {
            entity.HasIndex(p => p.Guid)
                  .IsUnique();

            entity.Property(x => x.Guid).HasDefaultValueSql("NEWID()");
        }
    }
}
