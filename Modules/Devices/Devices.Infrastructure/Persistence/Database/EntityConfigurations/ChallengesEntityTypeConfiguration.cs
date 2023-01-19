using Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class ChallengesEntityTypeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.ToTable(nameof(Challenge) + "s", "Challenges");

        builder.Property(x => x.Id).IsUnicode(false).IsFixedLength().HasMaxLength(20);
    }
}
