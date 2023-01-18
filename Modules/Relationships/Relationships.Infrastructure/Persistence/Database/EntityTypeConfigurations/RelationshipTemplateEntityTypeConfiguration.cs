using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;

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
