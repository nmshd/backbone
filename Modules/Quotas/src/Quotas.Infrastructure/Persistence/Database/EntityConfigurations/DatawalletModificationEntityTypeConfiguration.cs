using Backbone.Modules.Quotas.Domain.Aggregates.DatawalletModifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class DatawalletModificationEntityTypeConfiguration : IEntityTypeConfiguration<DatawalletModification>
{
    public void Configure(EntityTypeBuilder<DatawalletModification> builder)
    {
        builder.ToTable(nameof(DatawalletModification), "Synchronization", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
    }
}
