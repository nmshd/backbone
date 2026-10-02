using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetRelationship;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<Query, RelationshipId>();
    }
}
