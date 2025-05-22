using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Devices;

public class DeviceEntityTypeConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices", "Devices", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
        // builder.HasOne(x => x.User).WithOne(x => x.Device);
    }
}
