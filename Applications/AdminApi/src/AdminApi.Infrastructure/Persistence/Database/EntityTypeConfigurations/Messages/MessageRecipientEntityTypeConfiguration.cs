using Backbone.AdminApi.Infrastructure.Persistence.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Messages;

public class MessageRecipientEntityTypeConfiguration : IEntityTypeConfiguration<MessageRecipient>
{
    public void Configure(EntityTypeBuilder<MessageRecipient> builder)
    {
        builder.ToTable("RecipientInformation", "Messages", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
