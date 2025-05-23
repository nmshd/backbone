using Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Relationships;

public class RelationshipTemplateEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplate>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplate> builder)
    {
        builder.ToTable("RelationshipTemplates", "Relationships", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
