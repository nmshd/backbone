using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationship;

public class Validator : AbstractValidator<RevokeRelationshipCommand>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<RevokeRelationshipCommand, RelationshipId>();
    }
}
