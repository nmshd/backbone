using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class ChallengesEntityTypeConfiguration : EntityEntityTypeConfiguration<Challenge>
{
    public override void Configure(EntityTypeBuilder<Challenge> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(Challenge) + "s", "Challenges", x => x.ExcludeFromMigrations());

        builder.Property(x => x.Id).IsUnicode(false).IsFixedLength().HasMaxLength(20);
    }
}
