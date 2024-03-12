﻿using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;
public class Handler : IRequestHandler<CancelDeletionProcessCommand, CancelDeletionProcessResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<CancelDeletionProcessResponse> Handle(CancelDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var deviceId = _userContext.GetDeviceId();
        var deletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (deletionProcessIdResult.IsFailure)
            throw new DomainException(deletionProcessIdResult.Error);

        var deletionProcessId = deletionProcessIdResult.Value;

        var deletionProcess = identity.CancelDeletionProcess(deletionProcessId, deviceId);

        await _identitiesRepository.Update(identity, cancellationToken);

        return new CancelDeletionProcessResponse(deletionProcess);
    }
}
