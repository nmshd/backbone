using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CreateRelationship;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.RelationshipTemplateId).ValidId<Command, RelationshipTemplateId>();
        RuleFor(c => c.CreationContent).NumberOfBytes(0, 10.Mebibytes());
    }
}
