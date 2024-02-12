using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database;

public class TokensDbContext : AbstractDbContextBase
{
    public TokensDbContext() { }

    public TokensDbContext(DbContextOptions<TokensDbContext> options) : base(options) { }

    public TokensDbContext(DbContextOptions<TokensDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider) { }

    public virtual DbSet<Token> Tokens { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    base.OnConfiguring(optionsBuilder);
    //    optionsBuilder.UseSqlServer();
    //}

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<TokenId>().AreUnicode(false).AreFixedLength().HaveMaxLength(TokenId.MAX_LENGTH).HaveConversion<TokenIdEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(TokensDbContext).Assembly);
    }
}
