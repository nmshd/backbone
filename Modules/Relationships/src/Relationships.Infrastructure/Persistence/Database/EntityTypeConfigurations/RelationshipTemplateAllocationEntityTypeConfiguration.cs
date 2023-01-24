using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relationships.Domain.Entities;

namespace Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateAllocationEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplateAllocation>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplateAllocation> builder)
    {
        builder.ToTable(nameof(RelationshipTemplateAllocation) + "s");

        builder.HasKey(x => new { x.RelationshipTemplateId, x.AllocatedBy });
    }
}
