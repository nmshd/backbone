using Backbone.AdminApi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations;
public class RelationshipOverviewEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipOverview>
{
    public void Configure(EntityTypeBuilder<RelationshipOverview> builder)
    {
        builder.ToView("RelationshipOverviews");
        builder.HasNoKey();
    }
}
