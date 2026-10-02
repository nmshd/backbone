using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationshipReactivation;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<Command, RelationshipId>();
    }
}
