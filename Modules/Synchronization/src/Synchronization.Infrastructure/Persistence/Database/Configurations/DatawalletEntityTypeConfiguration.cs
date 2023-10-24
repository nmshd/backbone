﻿using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class DatawalletEntityTypeConfiguration : IEntityTypeConfiguration<Datawallet>
{
    public void Configure(EntityTypeBuilder<Datawallet> builder)
    {
        builder.HasIndex(p => p.Owner).IsUnique();

        builder.HasKey(x => x.Id);

        builder.HasMany(dw => dw.Modifications).WithOne(m => m.Datawallet);

        builder.Ignore(x => x.LatestModification);
    }
}
