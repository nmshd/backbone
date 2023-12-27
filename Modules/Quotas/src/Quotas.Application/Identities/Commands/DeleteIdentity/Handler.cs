using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
public class Handler(IIdentitiesRepository identitiesRepository) : IRequestHandler<DeleteIdentityCommand>
{
    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        await identitiesRepository.DeleteIdentities(Identity.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
