using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;

public class Handler : IRequestHandler<CancelStaleDeletionProcessesCommand, CancelStaleDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<CancelStaleDeletionProcessesResponse> Handle(CancelStaleDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identityRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, cancellationToken, true);

        var staleDeletionProcesses = new CancelStaleDeletionProcessesResponse();

        foreach (var identity in identities)
        {
            var staleDeletionProcess = identity.DeletionProcesses.First(dp=>dp.Status == DeletionProcessStatus.WaitingForApproval);

            if (staleDeletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime) >= DateTime.UtcNow) 
                continue;

            staleDeletionProcess.CancelAutomatically(identity.Address);
            staleDeletionProcesses.IdentityDeletionProcesses.Add(identity);

            await _identityRepository.Update(identity, cancellationToken);
        }

        return staleDeletionProcesses;
    }
}
