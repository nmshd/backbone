using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsOwner;

public class Validator : AbstractValidator<GetDeletionProcessAsOwnerQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetDeletionProcessAsOwnerQuery, IdentityDeletionProcessId>();
    }
}
