using AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class IdentityOverviewEntityTypeConfiguration : IEntityTypeConfiguration<IdentityOverviewDTO>
{
    public void Configure(EntityTypeBuilder<IdentityOverviewDTO> builder)
    {
        builder.ToView("IdentityOverviews");
        builder.HasKey(i => i.Address);
    }
}

