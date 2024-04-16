using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateAllocationEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplateAllocation>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplateAllocation> builder)
    {
        builder.ToTable(nameof(RelationshipTemplateAllocation) + "s");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.RelationshipTemplateId, x.AllocatedBy });
    }
}
