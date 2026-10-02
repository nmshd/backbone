using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.GetSyncRunById;

public class Validator : AbstractValidator<GetSyncRunByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.SyncRunId).ValidId<GetSyncRunByIdQuery, SyncRunId>();
    }
}
