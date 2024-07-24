using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;

public class Validator : AbstractValidator<GetDeletionProcessAsSupportQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<GetDeletionProcessAsSupportQuery, IdentityAddress>();
        RuleFor(x => x.DeletionProcessId).ValidId<GetDeletionProcessAsSupportQuery, IdentityDeletionProcessId>();
    }
}
