using Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Relationships;

public class RelationshipTemplateAllocationEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplateAllocation>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplateAllocation> builder)
    {
        builder.ToTable("RelationshipTemplateAllocations", "Relationships", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
