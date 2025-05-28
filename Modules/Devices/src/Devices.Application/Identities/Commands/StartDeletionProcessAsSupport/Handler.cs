using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;

public class Handler : IRequestHandler<StartDeletionProcessAsSupportCommand, StartDeletionProcessAsSupportResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<StartDeletionProcessAsSupportResponse> Handle(StartDeletionProcessAsSupportCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(request.IdentityAddress, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        await _identitiesRepository.Update(identity, cancellationToken);

        return new StartDeletionProcessAsSupportResponse(deletionProcess);
    }
}
