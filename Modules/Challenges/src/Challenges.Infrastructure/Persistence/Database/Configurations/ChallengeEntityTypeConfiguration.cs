﻿using Backbone.Challenges.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Challenges.Infrastructure.Persistence.Database.Configurations;

public class ChallengeEventEntityTypeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
    }
}
