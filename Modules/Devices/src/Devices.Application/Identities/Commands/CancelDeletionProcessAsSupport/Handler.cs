using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;

public class Handler : IRequestHandler<CancelDeletionAsSupportCommand, CancelDeletionAsSupportResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IEventBus _eventBus;

    public Handler(IIdentitiesRepository identitiesRepository, IEventBus eventBus)
    {
        _identitiesRepository = identitiesRepository;
        _eventBus = eventBus;
    }

    public async Task<CancelDeletionAsSupportResponse> Handle(CancelDeletionAsSupportCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.Address, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));
        var oldTierId = identity.TierId;

        var deletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (deletionProcessIdResult.IsFailure)
            throw new DomainException(deletionProcessIdResult.Error);

        var deletionProcessId = deletionProcessIdResult.Value;

        var deletionProcess = identity.CancelDeletionProcessAsSupport(deletionProcessId);

        await _identitiesRepository.Update(identity, cancellationToken);
        var newTierId = identity.TierId;

        _eventBus.Publish(new TierOfIdentityChangedIntegrationEvent(identity, oldTierId, newTierId));
        _eventBus.Publish(new IdentityDeletionProcessStatusChangedIntegrationEvent(identity.Address, deletionProcess.Id));

        return new CancelDeletionAsSupportResponse(deletionProcess);
    }
}
