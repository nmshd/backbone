using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsSupport;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<Query, IdentityAddress>();
        RuleFor(x => x.DeletionProcessId).ValidId<Query, IdentityDeletionProcessId>();
    }
}
