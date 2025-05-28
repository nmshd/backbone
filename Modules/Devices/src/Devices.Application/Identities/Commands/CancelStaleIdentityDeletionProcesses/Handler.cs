using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleIdentityDeletionProcesses;

public class Handler : IRequestHandler<CancelStaleIdentityDeletionProcessesCommand, CancelStaleIdentityDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<CancelStaleIdentityDeletionProcessesResponse> Handle(CancelStaleIdentityDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identityRepository.ListWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, cancellationToken, track: true);

        var idsOfCancelledDeletionProcesses = new List<IdentityDeletionProcessId>();

        foreach (var identity in identities)
        {
            var deletionProcess = identity.CancelStaleDeletionProcess();

            if (deletionProcess.IsFailure)
                continue;

            idsOfCancelledDeletionProcesses.Add(deletionProcess.Value.Id);

            await _identityRepository.Update(identity, cancellationToken);
        }

        return new CancelStaleIdentityDeletionProcessesResponse(idsOfCancelledDeletionProcesses);
    }
}
