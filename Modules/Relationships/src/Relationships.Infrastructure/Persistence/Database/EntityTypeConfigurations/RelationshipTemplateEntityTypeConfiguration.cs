using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplate>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplate> builder)
    {
        builder.HasIndex(x => x.CreatedBy);
        builder.HasIndex(x => x.DeletedAt);
        builder.HasIndex(x => x.ExpiresAt);

        builder
            .HasMany(x => x.Relationships)
            .WithOne(x => x.RelationshipTemplate)
            .HasForeignKey(x => x.RelationshipTemplateId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
