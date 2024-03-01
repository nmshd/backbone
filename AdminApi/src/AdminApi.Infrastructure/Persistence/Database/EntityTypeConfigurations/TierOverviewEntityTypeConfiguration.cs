using Backbone.AdminApi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class TierOverviewEntityTypeConfiguration : IEntityTypeConfiguration<TierOverview>
{
    public void Configure(EntityTypeBuilder<TierOverview> builder)
    {
        builder.ToView("TierOverviews");
        builder.HasNoKey();
    }
}
