﻿using Backbone.Modules.Challenges.Application.Infrastructure.Persistence;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database.ValueConverters;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence.Database;

public class ChallengesDbContext : AbstractDbContextBase, IChallengesDbContext
{
    public ChallengesDbContext() { }

    public ChallengesDbContext(DbContextOptions<ChallengesDbContext> options) : base(options) { }

    public virtual DbSet<Challenge> Challenges { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ChallengesDbContext).Assembly);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    base.OnConfiguring(optionsBuilder);
    //    optionsBuilder.UseSqlServer();
    //}

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<ChallengeId>().AreUnicode(false).AreFixedLength().HaveMaxLength(ChallengeId.MAX_LENGTH).HaveConversion<ChallengeIdEntityFrameworkValueConverter>();
    }
}
