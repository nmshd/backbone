using Backbone.Modules.Devices.Contracts;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;

public class IdentityStatusProvider(IMediator mediator) : IIdentityStatusProvider
{
    public async Task<bool> IsActive(string address, CancellationToken cancellationToken)
    {
        var identity = await mediator.Send(new GetIdentityQuery { Address = address }, cancellationToken);
        return identity.Status is IdentityStatus.Active;
    }
}
