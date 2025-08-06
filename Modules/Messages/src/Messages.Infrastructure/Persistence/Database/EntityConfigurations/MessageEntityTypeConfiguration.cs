using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class MessageEntityTypeConfiguration : EntityEntityTypeConfiguration<Message>, IEntityTypeConfiguration<MessageDetails>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.HasIndex(m => m.CreatedBy).HasMethod("hash");

        builder.Property(x => x.CreatedByDevice);
        builder.Property(x => x.CreatedAt);

        builder.HasOne(x => x.Details).WithOne().HasForeignKey<MessageDetails>(x => x.Id).IsRequired();

        builder.HasKey(m => m.Id);
    }

    public void Configure(EntityTypeBuilder<MessageDetails> builder)
    {
        builder.ToTable("Messages");
    }
}
