using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerStaleDeletionProcesses;

public class Handler : IRequestHandler<TriggerStaleDeletionProcessesCommand, TriggerStaleDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<TriggerStaleDeletionProcessesResponse> Handle(TriggerStaleDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identityRepository.FindByAddress(request.IdentityAddress, cancellationToken, true);

        var identityDeletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (identityDeletionProcessIdResult.IsFailure)
        {
            
        }

        var identityDeletionProcessId = identityDeletionProcessIdResult.Value;
        var deletionProcess = identity.CancelStaleDeletionProcess(identityDeletionProcessId);
        await _identityRepository.Update(identity, cancellationToken);

        return new TriggerStaleDeletionProcessesResponse(deletionProcess);
    }
}
