using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using CSharpFunctionalExtensions;
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
        var deletedIdentityAddresses = new Dictionary<IdentityAddress, UnitResult<DomainError>>();

        var identities = await _identitiesRepository.Find(Identity.IsReadyForDeletion(), cancellationToken, track: true);
        foreach (var identity in identities)
        {
            try
            {
                identity.DeletionStarted();
                await _identitiesRepository.Update(identity, cancellationToken);
                deletedIdentityAddresses.Add(identity.Address, UnitResult.Success<DomainError>());
            }
            catch (DomainException ex)
            {
                deletedIdentityAddresses.Add(identity.Address, UnitResult.Failure(ex.Error));
            }
        }

        return new TriggerRipeDeletionProcessesResponse(deletedIdentityAddresses);
    }
}
