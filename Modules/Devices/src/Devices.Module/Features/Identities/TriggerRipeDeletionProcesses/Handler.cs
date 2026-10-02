using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.TriggerRipeDeletionProcesses;

public class Handler : IRequestHandler<Command, Response>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.List(Identity.IsReadyForDeletion(), cancellationToken, track: true);

        var response = new Response();

        foreach (var identity in identities)
        {
            try
            {
                identity.DeletionStarted();
                await _identitiesRepository.Update(identity, cancellationToken);
                response.AddSuccess(identity.Address);
            }
            catch (DomainException ex)
            {
                response.AddError(identity.Address, ex.Error);
            }
        }

        return response;
    }
}
