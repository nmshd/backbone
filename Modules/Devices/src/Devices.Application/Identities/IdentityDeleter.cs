﻿using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteRegistrationsByIdentityAddress;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities;
public class IdentityDeleter(IMediator mediator) : IIdentityDeleter
{

    private readonly IMediator _mediator = mediator;
    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteRegistrationsByIdentityAddressCommand(identityAddress));
    }
}
