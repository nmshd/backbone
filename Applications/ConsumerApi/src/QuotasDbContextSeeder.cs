using Backbone.BuildingBlocks.API.Extensions;
using Backbone.Modules.Quotas.Application.Tiers.Commands.SeedQueuedForDeletionTier;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using MediatR;

namespace Backbone.ConsumerApi;

public class QuotasDbContextSeeder : IDbSeeder<QuotasDbContext>
{
    private readonly IMediator _mediator;

    public QuotasDbContextSeeder(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SeedAsync(QuotasDbContext context)
    {
        await SeedEverything(context);
    }

    private async Task SeedEverything(QuotasDbContext context)
    {
        await SeedQueuedForDeletionTierMetrics();
    }

    private async Task SeedQueuedForDeletionTierMetrics()
    {
        await _mediator.Send(new SeedQueuedForDeletionTierCommand());
    }
}
