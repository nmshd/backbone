﻿using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
internal class RelationshipAuditLogEntryEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipAuditLogEntry>
{
    public void Configure(EntityTypeBuilder<RelationshipAuditLogEntry> builder)
    {
        builder.ToTable(nameof(RelationshipAuditLogEntry), "RelationshipAuditLog", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason);
    }
}
