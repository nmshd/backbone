using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessGracePeriodReminders;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessDuringGracePeriod;
public class Handler
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task Handle(CancelDeletionProcessDuringGracePeriodCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        identity.CancelDeletionProcessDuringGracePeriod(identity.Devices[0].Id);

        await _identitiesRepository.Update(identity, cancellationToken);
    }
}
