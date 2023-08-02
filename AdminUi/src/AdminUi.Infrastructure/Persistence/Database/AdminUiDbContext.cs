using AdminUi.Infrastructure.DTOs;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace AdminUi.Infrastructure.Persistence.Database;

public class AdminUiDbContext : AbstractDbContextBase
{
    public AdminUiDbContext(DbContextOptions<AdminUiDbContext> options) : base(options) { }

    public DbSet<IdentityOverviewDTO> IdentityOverviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AdminUiDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeValueConverter>();
    }
}
