using Backbone.Modules.Quotas.Application.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database;

public class QuotasDbContext : AbstractDbContextBase
{
    public QuotasDbContext(DbContextOptions<QuotasDbContext> options): base(options)
    { }
}