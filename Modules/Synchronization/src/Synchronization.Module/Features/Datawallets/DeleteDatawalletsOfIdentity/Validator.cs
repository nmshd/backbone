using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.DeleteDatawalletsOfIdentity;

public class Validator : AbstractValidator<DeleteDatawalletsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeleteDatawalletsOfIdentityCommand, IdentityAddress>();
    }
}
