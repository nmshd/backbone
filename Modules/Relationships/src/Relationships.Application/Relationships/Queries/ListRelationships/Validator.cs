using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<ListRelationshipsQuery>
{
    public Validator()
    {
        RuleFor(q => q.Ids)
            .Cascade(CascadeMode.Stop)
            .DetailedNotNull()
            .Must(ids => ids.Count > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).WithMessage("'Ids' must not be empty.");

        RuleForEach(x => x.Ids).ValidId<ListRelationshipsQuery, RelationshipId>();
    }
}
