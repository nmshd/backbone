using AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;
public class ClientOverviewEntityTypeConfiguration : IEntityTypeConfiguration<ClientOverview>
{
    public void Configure(EntityTypeBuilder<ClientOverview> builder)
    {
        builder.ToView("ClientOverviews");
        builder.HasNoKey();
    }
}
