using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

// ReSharper disable once UnusedMember.Global
public class FinalizeDatawalletVersionUpgradeSyncRunCommandValidator : AbstractValidator<FinalizeDatawalletVersionUpgradeSyncRunCommand>
{
    public FinalizeDatawalletVersionUpgradeSyncRunCommandValidator()
    {
        RuleFor(x => x.SyncRunId).ValidId<FinalizeDatawalletVersionUpgradeSyncRunCommand, SyncRunId>();
        RuleFor(x => x.NewDatawalletVersion).DetailedNotEmpty();
        RuleForEach(x => x.DatawalletModifications).SetValidator(new PushDatawalletModificationItemValidator());
    }
}
