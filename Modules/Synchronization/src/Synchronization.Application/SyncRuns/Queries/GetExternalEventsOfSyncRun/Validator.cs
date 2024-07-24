using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;

public class Validator : AbstractValidator<GetExternalEventsOfSyncRunQuery>
{
    public Validator()
    {
        RuleFor(x => x.SyncRunId).ValidId<GetExternalEventsOfSyncRunQuery, SyncRunId>();
    }
}
