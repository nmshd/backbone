using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database;

public class MessagesDbContext : AbstractDbContextBase
{
    public MessagesDbContext()
    {
    }

    public MessagesDbContext(DbContextOptions<MessagesDbContext> options, IEventBus eventBus) : base(options, eventBus)
    {
    }

    public MessagesDbContext(DbContextOptions<MessagesDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options, eventBus, serviceProvider)
    {
    }

    public virtual DbSet<Message> Messages { get; set; } = null!;
    public virtual DbSet<RecipientInformation> RecipientInformation { get; set; } = null!;
    public virtual DbSet<Relationship> Relationships { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<FileId>().AreUnicode(false).AreFixedLength().HaveMaxLength(FileId.MAX_LENGTH).HaveConversion<FileIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<MessageId>().AreUnicode(false).AreFixedLength().HaveMaxLength(MessageId.MAX_LENGTH).HaveConversion<MessageIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<RelationshipId>().AreUnicode(false).AreFixedLength().HaveMaxLength(RelationshipId.MAX_LENGTH).HaveConversion<RelationshipIdEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Messages");

        builder.ApplyConfigurationsFromAssembly(typeof(MessagesDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        // Note: That option raises an exception when multiple collections are included in a query. It should help while debugging to
        // find out where the issue is. In case of such exception you should use the .AsSplitQuery() method to split the query into
        // multiple queries. See: https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries#split-queries
        optionsBuilder.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
#endif
        base.OnConfiguring(optionsBuilder);
    }
}
