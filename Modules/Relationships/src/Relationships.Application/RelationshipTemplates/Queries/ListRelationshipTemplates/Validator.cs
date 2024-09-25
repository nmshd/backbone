using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
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
        RuleFor(q => q.Queries)
            .Cascade(CascadeMode.Stop)
            .DetailedNotNull()
            .Must(queries => queries.Count > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).WithMessage("'Queries' must not be empty.");

        RuleForEach(x => x.Queries)
            .ValidRelationshipTemplate<ListRelationshipTemplatesQuery, RelationshipTemplateQuery, RelationshipTemplateId>(
                query => query.Id,
                query => query.Password
            );
    }
}
