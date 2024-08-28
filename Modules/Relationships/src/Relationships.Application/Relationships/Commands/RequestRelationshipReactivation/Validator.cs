using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RequestRelationshipReactivation;

public class Validator : AbstractValidator<RequestRelationshipReactivationCommand>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<RequestRelationshipReactivationCommand, RelationshipId>();
    }
}
