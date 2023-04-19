using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class QuotasEntityTypeConfiguration : IEntityTypeConfiguration<Quota>
{
    public void Configure(EntityTypeBuilder<Quota> builder)
    {
    }
}
