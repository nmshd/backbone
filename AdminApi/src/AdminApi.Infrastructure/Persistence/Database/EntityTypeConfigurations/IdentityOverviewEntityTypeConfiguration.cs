using Backbone.AdminApi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class IdentityOverviewEntityTypeConfiguration : IEntityTypeConfiguration<IdentityOverview>
{
    public void Configure(EntityTypeBuilder<IdentityOverview> builder)
    {
        builder.ToView("IdentityOverviews");
        builder.OwnsOne(
            c => c.Tier,
            dt =>
            {
                dt.Property(p => p.Id).HasColumnName("TierId");
                dt.Property(p => p.Name).HasColumnName("TierName");
            });
        builder.HasKey(c => c.Address);
    }
}

