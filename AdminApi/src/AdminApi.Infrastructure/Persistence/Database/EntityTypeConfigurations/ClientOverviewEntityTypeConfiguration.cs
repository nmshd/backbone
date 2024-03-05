using Backbone.AdminApi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations;
public class ClientOverviewEntityTypeConfiguration : IEntityTypeConfiguration<ClientOverview>
{
    public void Configure(EntityTypeBuilder<ClientOverview> builder)
    {
        builder.ToView("ClientOverviews");
        builder.OwnsOne(
            c => c.DefaultTier,
            dt =>
            {
                dt.Property(p => p.Id).HasColumnName("DefaultTierId");
                dt.Property(p => p.Name).HasColumnName("DefaultTierName");
            });
        builder.HasKey(c => c.ClientId);
    }
}
