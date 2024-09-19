using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
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
        await _identitiesRepository.Delete(Identity.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
