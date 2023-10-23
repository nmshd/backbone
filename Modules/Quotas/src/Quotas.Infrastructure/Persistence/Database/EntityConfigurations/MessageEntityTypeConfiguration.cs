using Backbone.Quotas.Domain.Aggregates.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable(nameof(Message) + "s", "Messages", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
    }
}
