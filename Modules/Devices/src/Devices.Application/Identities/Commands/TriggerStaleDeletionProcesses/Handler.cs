using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain;
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
        var identity = await _identityRepository.FindByAddress(request.IdentityAddress, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var identityDeletionProcessId = IdentityDeletionProcessId.Create(request.DeletionProcessId).Value;

        //var identityDeletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        //if (identityDeletionProcessIdResult.IsFailure)
        //    throw new DomainException(identityDeletionProcessIdResult.Error);

        //var identityDeletionProcessId = identityDeletionProcessIdResult.Value;
        var deletionProcess = identity.CancelStaleDeletionProcess(identityDeletionProcessId);
        await _identityRepository.Update(identity, cancellationToken);

        return new TriggerStaleDeletionProcessesResponse(deletionProcess);
    }
}
