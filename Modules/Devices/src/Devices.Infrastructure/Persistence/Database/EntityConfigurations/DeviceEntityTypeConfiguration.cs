using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class DeviceEntityTypeConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.Ignore(x => x.IsOnboarded);
    }
}
