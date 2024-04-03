using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class RecipientInformationEntityTypeConfiguration : IEntityTypeConfiguration<RecipientInformation>
{
    public void Configure(EntityTypeBuilder<RecipientInformation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(m => m.ReceivedAt);
        builder.HasIndex(r => new { r.Address, r.MessageId });

        builder
            .Property(r => r.EncryptedKey)
            .IsRequired();

        builder.HasOne<Relationship>().WithMany();
    }
}
