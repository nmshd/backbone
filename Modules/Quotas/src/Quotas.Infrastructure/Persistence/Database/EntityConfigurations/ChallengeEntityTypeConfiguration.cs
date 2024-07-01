using Backbone.Modules.Quotas.Domain.Aggregates.Challenges;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class ChallengeEntityTypeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.ToTable($"{nameof(Challenge)}s", "Challenges", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
    }
}
