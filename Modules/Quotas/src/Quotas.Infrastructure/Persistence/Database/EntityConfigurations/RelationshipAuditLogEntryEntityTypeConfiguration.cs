using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
internal class RelationshipAuditLogEntryEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipAuditLogEntry>
{
    public void Configure(EntityTypeBuilder<RelationshipAuditLogEntry> builder)
    {
        builder.ToTable("RelationshipAuditLog", "Relationships", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason);
        builder.Property(x => x.CreatedAt);
    }
}
