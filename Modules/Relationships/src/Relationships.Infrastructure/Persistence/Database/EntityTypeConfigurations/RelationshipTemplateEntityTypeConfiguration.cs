using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relationships.Domain.Entities;

namespace Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplate>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplate> builder)
    {
        builder.HasIndex(x => x.CreatedBy);
        builder.HasIndex(x => x.DeletedAt);
        builder.HasIndex(x => x.ExpiresAt);

        builder.Ignore(x => x.Content);
    }
}
