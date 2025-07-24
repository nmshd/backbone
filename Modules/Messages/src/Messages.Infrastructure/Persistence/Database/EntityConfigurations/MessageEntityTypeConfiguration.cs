using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class MessageEntityTypeConfiguration : EntityEntityTypeConfiguration<Message>, IEntityTypeConfiguration<MessageBody>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.HasIndex(m => m.CreatedBy).HasMethod("hash");

        builder.HasOne<MessageBody>(x => x.Body).WithOne().HasForeignKey<Message>(r => r.Id).OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedByDevice);
        builder.Property(x => x.CreatedAt);

        builder.HasKey(m => m.Id);
    }

    public void Configure(EntityTypeBuilder<MessageBody> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Body);
    }
}
