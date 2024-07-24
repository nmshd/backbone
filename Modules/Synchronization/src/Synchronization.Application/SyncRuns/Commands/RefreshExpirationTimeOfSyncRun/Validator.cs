using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class Validator : AbstractValidator<RefreshExpirationTimeOfSyncRunCommand>
{
    public Validator()
    {
        RuleFor(x => x.SyncRunId).ValidId<RefreshExpirationTimeOfSyncRunCommand, SyncRunId>();
    }
}
