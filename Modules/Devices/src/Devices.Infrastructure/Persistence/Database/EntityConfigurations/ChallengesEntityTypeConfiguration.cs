using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class ChallengesEntityTypeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.ToTable(nameof(Challenge) + "s", "Challenges", x => x.ExcludeFromMigrations());

        builder.Property(x => x.Id).IsUnicode(false).IsFixedLength().HasMaxLength(20);
    }
}
