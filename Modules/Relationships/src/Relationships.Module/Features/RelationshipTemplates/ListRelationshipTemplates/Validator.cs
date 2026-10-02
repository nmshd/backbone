using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<ListRelationshipTemplatesQuery>
{
    public Validator()
    {
        RuleFor(q => q.Ids)
            .Cascade(CascadeMode.Stop)
            .DetailedNotEmpty();

        RuleForEach(x => x.Ids)
            .Cascade(CascadeMode.Stop)
            .ValidId<ListRelationshipTemplatesQuery, RelationshipTemplateId>();
    }
}
