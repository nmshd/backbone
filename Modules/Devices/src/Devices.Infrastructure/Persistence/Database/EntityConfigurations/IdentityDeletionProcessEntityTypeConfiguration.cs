using System.Text.Json;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityDeletionProcessEntityTypeConfiguration : EntityEntityTypeConfiguration<IdentityDeletionProcess>
{
    public override void Configure(EntityTypeBuilder<IdentityDeletionProcess> builder)
    {
        base.Configure(builder);
        builder.ToTable("IdentityDeletionProcesses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status);
        builder.Property(x => x.CreatedAt);

        builder.HasIndex("IdentityAddress"); // we have to explicitly define the default foreign-key index because otherwise the filtered index below overrides the default index
        builder.HasIndex(["IdentityAddress"], "IX_only_one_active_deletion_process").IsUnique().HasFilter(@"""Status"" = 1");

        builder.HasMany(x => x.AuditLog).WithOne().OnDelete(DeleteBehavior.SetNull).IsRequired(false);
        builder.Ignore(x => x.HasGracePeriodExpired);
    }
}

public class IdentityDeletionProcessAuditLogEntryEntityTypeConfiguration : EntityEntityTypeConfiguration<IdentityDeletionProcessAuditLogEntry>
{
    public override void Configure(EntityTypeBuilder<IdentityDeletionProcessAuditLogEntry> builder)
    {
        base.Configure(builder);
        builder.ToTable("IdentityDeletionProcessAuditLog");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DeviceIdHash);
        builder.Property(x => x.IdentityAddressHash);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.MessageKey);
        builder.Property(x => x.NewStatus);
        builder.Property(x => x.OldStatus);
        builder.Property(x => x.MessageKey).HasConversion<string>();
        builder.Property(x => x.AdditionalData).HasConversion<string>(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default)
        );
    }
}
