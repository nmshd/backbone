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
        var identities = await _identityRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, cancellationToken, true); 

        var staleDeletionProcesses = new TriggerStaleDeletionProcessesResponse();

        foreach (var identity in identities)
        {
            foreach (var identityDeletionProcess in identity.DeletionProcesses)
            {
                if (identityDeletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime) < DateTime.UtcNow)
                {
                    staleDeletionProcesses.IdentityDeletionProcesses.Add(identity);
                }
            }
        }

        return staleDeletionProcesses;
    }
}
