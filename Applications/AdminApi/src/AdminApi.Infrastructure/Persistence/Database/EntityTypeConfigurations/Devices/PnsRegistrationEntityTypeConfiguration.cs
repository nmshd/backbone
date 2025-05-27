using Backbone.AdminApi.Infrastructure.Persistence.Database.Converters;
using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Devices;

public class PnsRegistrationEntityTypeConfiguration : IEntityTypeConfiguration<PnsRegistration>
{
    public void Configure(EntityTypeBuilder<PnsRegistration> builder)
    {
        builder.ToTable("PnsRegistrations", "Devices", t => t.ExcludeFromMigrations());

        builder.Property(r => r.Handle).HasConversion<PnsHandleEntityFrameworkValueConverter>();

        builder.HasKey(x => x.DeviceId);
    }
}
