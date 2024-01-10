using Backbone.AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;
public class MessageRecipientEntityTypeConfiguration : IEntityTypeConfiguration<MessageRecipient>
{
    public void Configure(EntityTypeBuilder<MessageRecipient> builder)
    {
        builder.ToView("MessageRecipients");
        builder.HasKey(r => new { r.Address, r.MessageId});
    }
}
