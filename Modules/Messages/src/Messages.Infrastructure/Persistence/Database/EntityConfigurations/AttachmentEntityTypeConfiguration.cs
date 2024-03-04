using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class AttachmentEntityTypeConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable($"{nameof(Attachment)}s");

        builder
            .HasKey(m => new { m.Id, m.MessageId });
    }
}
