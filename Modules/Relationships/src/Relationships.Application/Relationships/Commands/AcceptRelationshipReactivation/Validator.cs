using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipReactivation;

public class Validator : AbstractValidator<AcceptRelationshipReactivationCommand>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<AcceptRelationshipReactivationCommand, RelationshipId>();
    }
}
