using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence.Database;

public class ChallengesDbContext : AbstractDbContextBase
{
    public ChallengesDbContext()
    {
    }

    public ChallengesDbContext(DbContextOptions<ChallengesDbContext> options, IEventBus eventBus) : base(options, eventBus)
    {
    }

    public ChallengesDbContext(DbContextOptions<ChallengesDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options, eventBus, serviceProvider)
    {
    }

    public virtual DbSet<Challenge> Challenges { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Challenges");

        builder.ApplyConfigurationsFromAssembly(typeof(ChallengesDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<ChallengeId>().AreUnicode(false).AreFixedLength().HaveMaxLength(ChallengeId.MAX_LENGTH).HaveConversion<ChallengeIdEntityFrameworkValueConverter>();
    }
}
