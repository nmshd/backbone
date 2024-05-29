using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class AttachmentEntityTypeConfiguration : EntityEntityTypeConfiguration<Attachment>
{
    public override void Configure(EntityTypeBuilder<Attachment> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(Attachment)}s");

        builder
            .HasKey(m => new { m.Id, m.MessageId });
    }
}
