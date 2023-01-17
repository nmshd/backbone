using Challenges.Domain.Entities;
using Challenges.Domain.Ids;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challenges.Infrastructure.Persistence.Database.Configurations;

public class ChallengeEventEntityTypeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
    }
}
