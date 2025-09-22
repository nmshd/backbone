using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.ListExternalEventsOfSyncRun;

public class Validator : AbstractValidator<ListExternalEventsOfSyncRunQuery>
{
    public Validator()
    {
        RuleFor(x => x.SyncRunId).ValidId<ListExternalEventsOfSyncRunQuery, SyncRunId>();
    }
}
