﻿using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
public class Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger) : IRequestHandler<UpdateDeletionProcessesCommand, UpdateDeletionProcessesResponse>
{
    public async Task<UpdateDeletionProcessesResponse> Handle(UpdateDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateDeletionProcessesResponse
        {
            IdentityAddresses = []
        };

        var identities = await identitiesRepository.FindAllActiveWithPastDeletionGracePeriod(cancellationToken, track: true);
        foreach (var identity in identities)
        {
            try
            {
                identity.DeletionStarted();
                await identitiesRepository.Update(identity, cancellationToken);
                response.IdentityAddresses.Add(identity.Address);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity with PastDeletionGracePeriod did not have any active deletionProcesses. Identity Address: {address}", identity.Address);
            }
        }

        return response;
    }
}
