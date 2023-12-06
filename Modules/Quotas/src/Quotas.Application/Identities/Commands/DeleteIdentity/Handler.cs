using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
public class Handler(IIdentitiesRepository identitiesRepository) : IRequestHandler<DeleteIdentityCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository = identitiesRepository;

    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        // Deletion of Individual Quotas, Tier Quotas and MetricStatuses is ensured by Cascade Deletion of Foreign Key 
        await _identitiesRepository.DeleteIdentities(Identity.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
