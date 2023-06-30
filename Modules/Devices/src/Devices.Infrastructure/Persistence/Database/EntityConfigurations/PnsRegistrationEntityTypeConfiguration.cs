using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;
public class PnsRegistrationEntityTypeConfiguration : IEntityTypeConfiguration<PnsRegistration>
{
    public void Configure(EntityTypeBuilder<PnsRegistration> builder)
    {
        builder.ToTable("PnsRegistration");

        builder.HasKey(x => x.Handle);
        builder.Property(x => x.Handle).HasMaxLength(200);
        builder.Property(x => x.IdentityAddress);
        builder.Property(x => x.DeviceId);
        builder.Property(x => x.UpdatedAt);
    }
}
