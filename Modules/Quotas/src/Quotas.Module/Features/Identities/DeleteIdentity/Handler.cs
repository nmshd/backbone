using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.DeleteIdentity;

public class Handler : IRequestHandler<Command>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _identitiesRepository.Delete(Identity.HasAddress(request.IdentityAddress), cancellationToken);
    }
}
