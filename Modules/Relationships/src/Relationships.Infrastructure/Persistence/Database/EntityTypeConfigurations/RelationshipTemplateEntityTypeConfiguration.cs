using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Relationships.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateEntityTypeConfiguration : EntityEntityTypeConfiguration<RelationshipTemplate>
{
    public override void Configure(EntityTypeBuilder<RelationshipTemplate> builder)
    {
        base.Configure(builder);

        builder
            .HasMany(x => x.Relationships)
            .WithOne(x => x.RelationshipTemplate)
            .HasForeignKey(x => x.RelationshipTemplateId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.Allocations)
            .WithOne()
            .HasForeignKey(x => x.RelationshipTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
