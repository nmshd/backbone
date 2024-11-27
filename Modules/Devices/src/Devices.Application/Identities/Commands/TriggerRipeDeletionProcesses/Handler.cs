using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;

public class Handler : IRequestHandler<TriggerRipeDeletionProcessesCommand, TriggerRipeDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<TriggerRipeDeletionProcessesResponse> Handle(TriggerRipeDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.Find(Identity.IsReadyForDeletion(), cancellationToken, track: true);

        var response = new TriggerRipeDeletionProcessesResponse();

        foreach (var identity in identities)
        {
            try
            {
                identity.DeletionStarted();
                await _identitiesRepository.Update(identity, cancellationToken);
                response.AddSuccess(identity.Address);
            }
            catch (DomainException ex)
            {
                response.AddError(identity.Address, ex.Error);
            }
        }

        return response;
    }
}
