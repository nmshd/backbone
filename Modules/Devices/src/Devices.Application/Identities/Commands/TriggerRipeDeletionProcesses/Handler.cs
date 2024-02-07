using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
public class Handler : IRequestHandler<TriggerRipeDeletionProcessesCommand, TriggerRipeDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task<TriggerRipeDeletionProcessesResponse> Handle(TriggerRipeDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var response = new TriggerRipeDeletionProcessesResponse();

        var identities = await _identitiesRepository.FindAllToBeDeletedWithPastDeletionGracePeriod(cancellationToken, track: true);
        foreach (var identity in identities)
        {
            try
            {
                identity.DeletionStarted();
                await _identitiesRepository.Update(identity, cancellationToken);
                response.DeletedIdentityAddresses.Add(identity.Address);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "There was an error while triggering a deletion process.");
            }
        }

        return response;
    }
}
