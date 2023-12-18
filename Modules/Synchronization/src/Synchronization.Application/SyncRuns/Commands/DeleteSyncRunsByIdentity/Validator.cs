using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsByIdentity;
public class Validator : AbstractValidator<DeleteSyncRunsByIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
