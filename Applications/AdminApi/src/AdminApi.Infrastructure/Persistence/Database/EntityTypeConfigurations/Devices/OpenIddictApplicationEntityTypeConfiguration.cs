using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Devices;

public class OpenIddictApplicationEntityTypeConfiguration : IEntityTypeConfiguration<OpenIddictApplication>
{
    public void Configure(EntityTypeBuilder<OpenIddictApplication> builder)
    {
        builder.ToTable("OpenIddictApplications", "Devices", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
