using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;

public class Validator : AbstractValidator<RefreshExpirationTimeOfSyncRunCommand>
{
    public Validator()
    {
        RuleFor(x => x.SyncRunId).ValidId<RefreshExpirationTimeOfSyncRunCommand, SyncRunId>();
    }
}
