﻿using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities;

public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;
    private readonly IDeletionProcessLogger _deletionProcessLogger;

    public IdentityDeleter(IMediator mediator, IDeletionProcessLogger deletionProcessLogger)
    {
        _mediator = mediator;
        _deletionProcessLogger = deletionProcessLogger;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeletePnsRegistrationsOfIdentityCommand(identityAddress));
        await _deletionProcessLogger.LogDeletion(identityAddress, "PnsRegistrations");
        await _mediator.Send(new DeleteIdentityCommand(identityAddress));
        await _deletionProcessLogger.LogDeletion(identityAddress, "Identities");
    }
}
