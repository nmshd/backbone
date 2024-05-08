using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;

public class Validator : AbstractValidator<DeleteSyncRunsOfIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
