using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities;

public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;

    public IdentityDeleter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Delete(IdentityAddress identityAddress, IDeletionProcessLogger deletionProcessLogger)
    {
        await _mediator.Send(new DeletePnsRegistrationsOfIdentityCommand(identityAddress));
        await deletionProcessLogger.LogDeletion(identityAddress, "PnsRegistrations");
        await _mediator.Send(new DeleteIdentityCommand(identityAddress));
        await deletionProcessLogger.LogDeletion(identityAddress, "Identities");
    }
}
