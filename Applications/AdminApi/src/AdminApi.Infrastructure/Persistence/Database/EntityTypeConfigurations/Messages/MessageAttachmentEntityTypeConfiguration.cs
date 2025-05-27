using Backbone.AdminApi.Infrastructure.Persistence.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Messages;

public class MessageAttachmentEntityTypeConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("Attachments", "Messages", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
