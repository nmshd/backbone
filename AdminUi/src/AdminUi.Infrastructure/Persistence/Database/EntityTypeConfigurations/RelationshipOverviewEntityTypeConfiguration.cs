using Backbone.AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;
public class RelationshipOverviewEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipOverview>
{
    public void Configure(EntityTypeBuilder<RelationshipOverview> builder)
    {
        builder.ToView("RelationshipOverviews");
        builder.HasNoKey();
    }
}
