using AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class IdentityOverviewEntityTypeConfiguration : IEntityTypeConfiguration<IdentityOverview>
{
    public void Configure(EntityTypeBuilder<IdentityOverview> builder)
    {
        builder.ToView("IdentityOverviews");
        builder.HasNoKey();
    }
}

