using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Devices;

internal class AspNetUserEntityTypeConfiguration : IEntityTypeConfiguration<AspNetUser>
{
    public void Configure(EntityTypeBuilder<AspNetUser> builder)
    {
        builder.ToTable("AspNetUsers", "Devices", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
