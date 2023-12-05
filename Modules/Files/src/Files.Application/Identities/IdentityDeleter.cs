using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesByIdentity;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities;
public class IdentityDeleter(IMediator mediator) : IIdentityDeleter
{
    private readonly IMediator _mediator = mediator;

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteFilesByIdentityCommand(identityAddress));    
    }
}
