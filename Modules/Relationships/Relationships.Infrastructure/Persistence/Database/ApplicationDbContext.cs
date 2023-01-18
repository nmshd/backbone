using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;
using Relationships.Infrastructure.Persistence.Database.ValueConverters;

namespace Relationships.Infrastructure.Persistence.Database;

public class ApplicationDbContext : AbstractDbContextBase
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Relationship> Relationships { get; set; }
    public DbSet<RelationshipChange> RelationshipChanges { get; set; }
    public DbSet<RelationshipTemplate> RelationshipTemplates { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     base.OnConfiguring(optionsBuilder);
    //     optionsBuilder.UseSqlServer();
    // }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<RelationshipId>().AreUnicode(false).AreFixedLength().HaveMaxLength(RelationshipId.MAX_LENGTH).HaveConversion<RelationshipIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<RelationshipTemplateId>().AreUnicode(false).AreFixedLength().HaveMaxLength(RelationshipTemplateId.MAX_LENGTH).HaveConversion<RelationshipTemplateIdEntityFrameworkValueConverter>();

        // Uncommenting the following means that we would have to recreate the table on the database, which is why we decided to leave RelationshipChangeIds in nvarchar(20) for now.
        configurationBuilder.Properties<RelationshipChangeId>().HaveMaxLength(RelationshipChangeId.MAX_LENGTH).HaveConversion<RelationshipChangeIdEntityFrameworkValueConverter>(); //.AreFixedLength().AreUnicode(false)
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
