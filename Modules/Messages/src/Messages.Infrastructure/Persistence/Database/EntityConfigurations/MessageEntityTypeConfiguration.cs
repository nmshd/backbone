using Backbone.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasIndex(m => m.CreatedBy);
        builder.HasIndex(m => m.DoNotSendBefore);
        builder.HasIndex(m => m.CreatedAt);

        builder.Property(x => x.CreatedByDevice);

        builder
            .HasKey(m => m.Id);

        builder.Ignore(a => a.Body);
    }
}
