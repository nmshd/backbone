using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAsSupport;

public class Validator : AbstractValidator<GetDeletionProcessesAsSupportQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<GetDeletionProcessesAsSupportQuery, IdentityAddress>();
    }
}
