using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.ListExternalEventsOfSyncRun;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.SyncRunId).ValidId<Query, SyncRunId>();
    }
}
