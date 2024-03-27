using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipAuditLogEntryEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipAuditLogEntry>
{
    public void Configure(EntityTypeBuilder<RelationshipAuditLogEntry> builder)
    {
        builder.ToTable("RelationshipAuditLog");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.Reason);
        builder.Property(x => x.OldStatus);
        builder.Property(x => x.NewStatus);
        builder.Property(x => x.CreatedBy);
        builder.Property(x => x.CreatedByDevice);
    }
}
