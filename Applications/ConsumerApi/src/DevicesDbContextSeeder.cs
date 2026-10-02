using CreateQueuedForDeletionTier = Backbone.Modules.Devices.Module.Features.Tiers.CreateQueuedForDeletionTier;
using CreateTier = Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;
using SeedTestUsers = Backbone.Modules.Devices.Module.Features.Users.SeedTestUsers;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.Modules.Devices.Module.Features.Tiers.Shared;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using MediatR;

namespace Backbone.ConsumerApi;

public class DevicesDbContextSeeder : IDbSeeder<DevicesDbContext>
{
    private readonly IMediator _mediator;

    public DevicesDbContextSeeder(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SeedAsync(DevicesDbContext context)
    {
        await SeedEverything(context);
    }

    private async Task SeedEverything(DevicesDbContext context)
    {
        await SeedBasicTier(context);
        await SeedQueuedForDeletionTier();
        await SeedApplicationUsers();
    }

    private static async Task<Tier?> GetBasicTier(DevicesDbContext context)
    {
        return await context.Tiers.GetBasicTier(CancellationToken.None);
    }

    private async Task SeedApplicationUsers()
    {
        await _mediator.Send(new SeedTestUsers.Command());
    }

    private async Task SeedBasicTier(DevicesDbContext context)
    {
        if (await GetBasicTier(context) == null)
        {
            await _mediator.Send(new CreateTier.Command { Name = TierName.BASIC_DEFAULT_NAME });
        }
    }

    private async Task SeedQueuedForDeletionTier()
    {
        await _mediator.Send(new CreateQueuedForDeletionTier.Command());
    }
}
