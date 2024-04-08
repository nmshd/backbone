using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasIndex(m => m.CreatedBy);

        builder.Property(m => m.Body).IsRequired(false);
        builder.Property(x => x.CreatedByDevice);
        builder.Property(x => x.CreatedAt);

        builder.HasKey(m => m.Id);
    }
}
