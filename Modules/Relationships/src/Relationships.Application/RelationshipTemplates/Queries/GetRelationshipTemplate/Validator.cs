using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class Validator : AbstractValidator<GetRelationshipTemplateQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetRelationshipTemplateQuery, RelationshipTemplateId>();
    }
}
