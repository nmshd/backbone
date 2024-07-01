using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateAllocationEntityTypeConfiguration : EntityEntityTypeConfiguration<RelationshipTemplateAllocation>
{
    public override void Configure(EntityTypeBuilder<RelationshipTemplateAllocation> builder)
    {
        base.Configure(builder);

        base.Configure(builder);

        builder.ToTable(nameof(RelationshipTemplateAllocation) + "s");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.RelationshipTemplateId, x.AllocatedBy });
    }
}
