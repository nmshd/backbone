using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
public class Handler : IRequestHandler<RejectDeletionProcessCommand, RejectDeletionProcessResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<RejectDeletionProcessResponse> Handle(RejectDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));
        var deviceId = _userContext.GetDeviceId();

        var identityDeletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (identityDeletionProcessIdResult.IsFailure)
            throw new DomainException(identityDeletionProcessIdResult.Error);

        var identityDeletionProcessId = identityDeletionProcessIdResult.Value;
        var deletionProcess = identity.RejectDeletionProcess(identityDeletionProcessId, deviceId);
        await _identitiesRepository.Update(identity, cancellationToken);

        return new RejectDeletionProcessResponse(deletionProcess, identity.Address, deviceId);
    }
}
