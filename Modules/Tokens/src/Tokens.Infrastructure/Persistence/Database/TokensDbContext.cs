using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database;

public class TokensDbContext : AbstractDbContextBase
{
    public TokensDbContext()
    {
    }

    public TokensDbContext(DbContextOptions<TokensDbContext> options, IEventBus eventBus) : base(options, eventBus)
    {
    }

    public TokensDbContext(DbContextOptions<TokensDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options, eventBus, serviceProvider)
    {
    }

    public virtual DbSet<Token> Tokens { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<TokenId>().AreUnicode(false).AreFixedLength().HaveMaxLength(TokenId.MAX_LENGTH).HaveConversion<TokenIdEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Tokens");

        builder.ApplyConfigurationsFromAssembly(typeof(TokensDbContext).Assembly);
    }
}
