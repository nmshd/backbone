using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Command.DeleteRelationshipTemplate;

// ReSharper disable once UnusedMember.Global
public class DeleteRelationshipTemplateCommandValidator : AbstractValidator<DeleteRelationshipTemplateCommand>
{
    public DeleteRelationshipTemplateCommandValidator()
    {
        RuleFor(c => c.Id).DetailedNotNull();
    }
}
