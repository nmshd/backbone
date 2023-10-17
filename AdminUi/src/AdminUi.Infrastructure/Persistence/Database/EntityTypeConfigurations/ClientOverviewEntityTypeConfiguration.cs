using Backbone.AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;
public class ClientOverviewEntityTypeConfiguration : IEntityTypeConfiguration<ClientOverview>
{
    public void Configure(EntityTypeBuilder<ClientOverview> builder)
    {
        builder.ToView("ClientOverviews");
        builder.HasNoKey();
    }
}
