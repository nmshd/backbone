using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAsSupport;

public class Validator : AbstractValidator<ListDeletionProcessesAsSupportQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<ListDeletionProcessesAsSupportQuery, IdentityAddress>();
    }
}
