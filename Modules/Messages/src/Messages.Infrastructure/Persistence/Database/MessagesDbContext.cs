using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Messages.Domain.Entities;
using Backbone.Messages.Domain.Ids;
using Backbone.Messages.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Messages.Infrastructure.Persistence.Database;

public class MessagesDbContext : AbstractDbContextBase
{
    public MessagesDbContext() { }

    public MessagesDbContext(DbContextOptions<MessagesDbContext> options) : base(options) { }

    public MessagesDbContext(DbContextOptions<MessagesDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider) { }

    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<RecipientInformation> RecipientInformation { get; set; }
    public virtual DbSet<Relationship> Relationships { get; set; }

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

        builder.ApplyConfigurationsFromAssembly(typeof(MessagesDbContext).Assembly);
    }
}
