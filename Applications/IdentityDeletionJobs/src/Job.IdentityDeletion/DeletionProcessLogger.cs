using LogDeletionProcess = Backbone.Modules.Devices.Module.Features.Identities.LogDeletionProcess;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Job.IdentityDeletion;

public class DeletionProcessLogger : IDeletionProcessLogger
{
    private readonly IMediator _mediator;

    public DeletionProcessLogger(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task LogDeletion(IdentityAddress identityAddress, string aggregateType)
    {
        await _mediator.Send(new LogDeletionProcess.Command { IdentityAddress = identityAddress, AggregateType = aggregateType });
    }
}
