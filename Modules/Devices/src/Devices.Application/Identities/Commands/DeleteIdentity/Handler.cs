using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
public class Handler(IIdentitiesRepository identitiesRepository) : IRequestHandler<DeleteIdentityCommand>
{
    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        var identity = await identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken, track: true);
        await identitiesRepository.Delete(identity, cancellationToken);
    }
}
