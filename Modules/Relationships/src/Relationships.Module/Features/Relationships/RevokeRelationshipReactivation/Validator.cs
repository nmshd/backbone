using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationshipReactivation;

public class Validator : AbstractValidator<RevokeRelationshipReactivationCommand>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<RevokeRelationshipReactivationCommand, RelationshipId>();
    }
}
