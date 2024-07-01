using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class RecipientInformationEntityTypeConfiguration : EntityEntityTypeConfiguration<RecipientInformation>
{
    public override void Configure(EntityTypeBuilder<RecipientInformation> builder)
    {
        base.Configure(builder);

        builder.HasKey(r => r.Id);

        builder.HasIndex(m => m.ReceivedAt);
        builder.HasIndex(r => new { r.Address, r.MessageId });

        builder
            .Property(r => r.EncryptedKey)
            .IsRequired();

        builder.HasOne<Relationship>().WithMany();
    }
}
