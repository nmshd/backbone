using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;

public class Validator : AbstractValidator<GetRelationshipQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).Must(RelationshipId.IsValid);
    }
}
