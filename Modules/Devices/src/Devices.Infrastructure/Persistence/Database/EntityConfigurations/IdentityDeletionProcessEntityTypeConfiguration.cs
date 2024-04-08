using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityDeletionProcessEntityTypeConfiguration : IEntityTypeConfiguration<IdentityDeletionProcess>
{
    public void Configure(EntityTypeBuilder<IdentityDeletionProcess> builder)
    {
        builder.ToTable("IdentityDeletionProcesses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.ApprovalReminder1SentAt);
        builder.Property(x => x.ApprovalReminder2SentAt);
        builder.Property(x => x.ApprovalReminder3SentAt);

        builder.Ignore(x => x.HasApprovalPeriodExpired);
    }
}

public class IdentityDeletionProcessAuditLogEntryEntityTypeConfiguration : IEntityTypeConfiguration<IdentityDeletionProcessAuditLogEntry>
{
    public void Configure(EntityTypeBuilder<IdentityDeletionProcessAuditLogEntry> builder)
    {
        builder.ToTable("IdentityDeletionProcessAuditLog");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DeviceIdHash);
        builder.Property(x => x.IdentityAddressHash);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.Message);
        builder.Property(x => x.NewStatus);
        builder.Property(x => x.OldStatus);
    }
}
