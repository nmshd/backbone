﻿using Backbone.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class RecipientInformationEntityTypeConfiguration : IEntityTypeConfiguration<RecipientInformation>
{
    public void Configure(EntityTypeBuilder<RecipientInformation> builder)
    {
        builder
            .HasKey(r => new { r.Address, r.MessageId });

        builder.HasIndex(m => m.ReceivedAt);
        builder.HasIndex(m => m.RelationshipId);

        builder
            .Property(r => r.EncryptedKey)
            .IsRequired();

        builder.HasOne<Relationship>().WithMany();
    }
}
