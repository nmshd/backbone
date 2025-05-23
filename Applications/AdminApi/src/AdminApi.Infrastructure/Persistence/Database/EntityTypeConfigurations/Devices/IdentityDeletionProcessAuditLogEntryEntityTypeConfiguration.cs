using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Devices;

public class IdentityDeletionProcessAuditLogEntryEntityTypeConfiguration : IEntityTypeConfiguration<IdentityDeletionProcessAuditLogEntry>
{
    public void Configure(EntityTypeBuilder<IdentityDeletionProcessAuditLogEntry> builder)
    {
        builder.ToTable("IdentityDeletionProcessAuditLog", "Devices", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
