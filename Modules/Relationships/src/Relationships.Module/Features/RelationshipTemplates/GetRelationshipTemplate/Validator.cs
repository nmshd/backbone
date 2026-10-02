using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.GetRelationshipTemplate;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<Query, RelationshipTemplateId>();

        RuleFor(x => x.Password).NumberOfBytes(1, RelationshipTemplate.MAX_PASSWORD_LENGTH).When(t => t.Password != null);
    }
}
