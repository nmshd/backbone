using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<CreateRelationshipCommand>
{
    public Validator()
    {
        RuleFor(c => c.RelationshipTemplateId).ValidId<CreateRelationshipCommand, RelationshipTemplateId>();
        RuleFor(c => c.CreationContent).NumberOfBytes(0, 10.Mebibytes());
    }
}
