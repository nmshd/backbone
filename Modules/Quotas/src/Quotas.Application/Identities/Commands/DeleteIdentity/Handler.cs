using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
public class Handler : IRequestHandler<DeleteIdentityCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }
    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Find(request.IdentityAddress, cancellationToken);

        await _identitiesRepository.Delete(identity, cancellationToken);

        // --- delete all Individual Quotas
        // --- delete all Tier Quotas
        // --- delete all MetricStatuses
    }
}
