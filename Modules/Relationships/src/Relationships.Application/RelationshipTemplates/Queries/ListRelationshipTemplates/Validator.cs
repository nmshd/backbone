using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<ListRelationshipTemplatesQuery>
{
    public Validator()
    {
        RuleFor(q => q.QueryItems)
            .Cascade(CascadeMode.Stop)
            .DetailedNotEmpty();

        RuleForEach(x => x.QueryItems)
            .Cascade(CascadeMode.Stop)
            .ChildRules(queryItems =>
            {
                queryItems
                    .RuleFor(query => query.Id)
                    .ValidId<RelationshipTemplateQueryItem, RelationshipTemplateId>();

                queryItems
                    .RuleFor(query => query.Password)
                    .NumberOfBytes(1, 200)
                    .When(query => query.Password != null);
            });
    }
}
