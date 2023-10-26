using Backbone.AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class TierOverviewEntityTypeConfiguration : IEntityTypeConfiguration<TierOverview>
{
    public void Configure(EntityTypeBuilder<TierOverview> builder)
    {
        builder.ToView("TierOverviews");
        builder.HasNoKey();
    }
}
