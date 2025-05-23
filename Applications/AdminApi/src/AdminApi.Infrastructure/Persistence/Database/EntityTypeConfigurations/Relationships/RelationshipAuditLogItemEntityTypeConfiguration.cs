using Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Relationships;

public class RelationshipAuditLogItemEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipAuditLogItem>
{
    public void Configure(EntityTypeBuilder<RelationshipAuditLogItem> builder)
    {
        builder.ToTable("RelationshipAuditLog", "Relationships", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
