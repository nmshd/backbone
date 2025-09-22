using Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Synchronization;

public class ExternalEventEntityTypeConfiguration : IEntityTypeConfiguration<ExternalEvent>
{
    public void Configure(EntityTypeBuilder<ExternalEvent> builder)
    {
        builder.ToTable("ExternalEvents", "Synchronization", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
