using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeDatawalletVersionUpgrade;

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
