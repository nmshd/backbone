using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

// ReSharper disable once UnusedMember.Global
public class CreateRelationshipCommandValidator : AbstractValidator<CreateRelationshipCommand>
{
    public CreateRelationshipCommandValidator()
    {
        RuleFor(c => c.RelationshipTemplateId).DetailedNotEmpty();
        RuleFor(c => c.CreationContent).NumberOfBytes(0, 10.Mebibytes());
    }
}
