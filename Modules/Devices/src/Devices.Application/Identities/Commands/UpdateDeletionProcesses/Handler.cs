using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
public class Handler : IRequestHandler<UpdateDeletionProcessesCommand, UpdateDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IPnsRegistrationRepository _pnsRegistrationRepository;

    public Handler(IIdentitiesRepository identitiesRepository, IPnsRegistrationRepository pnsRegistrationRepository, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
        _pnsRegistrationRepository = pnsRegistrationRepository;
    }

    public async Task<UpdateDeletionProcessesResponse> Handle(UpdateDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateDeletionProcessesResponse
        {
            IdentityAddresses = new()
        };

        var identities = await _identitiesRepository.FindAllWithPastDeletionGracePeriod(cancellationToken, track: true);
        foreach (var identity in identities)
        {
            try
            {
                identity.DeletionStarted();
                await _identitiesRepository.Update(identity, cancellationToken);
                response.IdentityAddresses.Add(identity.Address);

                await _pnsRegistrationRepository.DeleteByIdentityAddress(identity.Address, cancellationToken); // pnsRegistrations ✅


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity with PastDeletionGracePeriod did not have any active deletionProcesses. Identity Address: {address}", identity.Address);
            }
        }

        return response;
    }
}
