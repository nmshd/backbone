using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsOwner;

public class Validator : AbstractValidator<GetDeletionProcessAsOwnerQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetDeletionProcessAsOwnerQuery, IdentityDeletionProcessId>();
    }
}
