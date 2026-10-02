using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.SyncRunId).ValidId<Command, SyncRunId>();
        RuleForEach(x => x.ExternalEventResults).SetValidator(new EventResultValidator());
        RuleForEach(x => x.DatawalletModifications).SetValidator(new PushDatawalletModificationItemValidator());
    }

    public class EventResultValidator : AbstractValidator<Command.ExternalEventResult>
    {
        public EventResultValidator()
        {
            RuleFor(i => i.ExternalEventId).ValidId<Command.ExternalEventResult, ExternalEventId>();
            RuleFor(i => i.ErrorCode).MaximumLength(100);
        }
    }
}
