using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
public class Handler : IRequestHandler<UpdateDeletionProcessesCommand, UpdateDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task<UpdateDeletionProcessesResponse> Handle(UpdateDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateDeletionProcessesResponse
        {
            IdentityAddresses = []
        };

        var identities = await _identitiesRepository.FindAllWithPastDeletionGracePeriod(cancellationToken, track: true);
        foreach (var identity in identities)
        {
            try
            {
                identity.DeletionStarted();
                await _identitiesRepository.Update(identity, cancellationToken);
                response.IdentityAddresses.Add(identity.Address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity with PastDeletionGracePeriod did not have any active deletionProcesses. Identity Address: {address}", identity.Address);
            }
        }

        return response;
    }
}
