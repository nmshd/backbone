using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
public class Handler : IRequestHandler<UpdateDeletionProcessesCommand, UpdateDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<UpdateDeletionProcessesResponse> Handle(UpdateDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateDeletionProcessesResponse();
        response.IdentityAddresses = new();

        var identities = await _identitiesRepository.FindAllWithPastDeletionGracePeriod(cancellationToken, track: true);
        foreach (var identity in identities)
        {
            identity.DeletionStarted();
            await _identitiesRepository.Update(identity, cancellationToken);
            
            response.IdentityAddresses.Add(identity.Address);
        }

        return response;
    }
}
