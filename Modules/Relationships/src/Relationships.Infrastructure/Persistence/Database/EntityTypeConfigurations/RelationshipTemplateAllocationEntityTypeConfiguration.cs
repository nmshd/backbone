using Backbone.Relationships.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateAllocationEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplateAllocation>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplateAllocation> builder)
    {
        builder.ToTable(nameof(RelationshipTemplateAllocation) + "s");

        builder.HasKey(x => new { x.RelationshipTemplateId, x.AllocatedBy });
    }
}
