using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Quotas.Domain.Aggregates.StartedDeletionProcesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
public class IdentityDeletionProcessesEntityTypeConfiguration : IEntityTypeConfiguration<IdentityDeletionProcesses>
{
    public void Configure(EntityTypeBuilder<IdentityDeletionProcesses> builder)
    {
        builder.ToTable(nameof(IdentityDeletionProcesses), "IdentityDeletionProcesses", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
        builder.HasKey(x => x.Status);
    }
}
