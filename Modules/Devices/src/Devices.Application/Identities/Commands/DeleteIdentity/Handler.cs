using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateAuditLog;
public class Handler(IIdentitiesRepository identitiesRepository) : IRequestHandler<DeleteIdentityCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository = identitiesRepository;

    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken);
        await _identitiesRepository.Delete(identity, cancellationToken);
    }
}
