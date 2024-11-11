using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplate;

public class Validator : AbstractValidator<DeleteRelationshipTemplateCommand>
{
    public Validator()
    {
        RuleFor(command => command.Id).ValidId<DeleteRelationshipTemplateCommand, RelationshipTemplateId>();
    }
}
