using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteExternalEventsOfIdentity;

public class Validator : AbstractValidator<DeleteExternalEventsOfIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
