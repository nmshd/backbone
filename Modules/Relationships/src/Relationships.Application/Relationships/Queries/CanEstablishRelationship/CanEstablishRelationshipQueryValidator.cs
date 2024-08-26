using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;

// ReSharper disable once UnusedType.Global
public class CanEstablishRelationshipQueryValidator : AbstractValidator<CanEstablishRelationshipQuery>
{
    public CanEstablishRelationshipQueryValidator()
    {
        RuleFor(q => q.PeerAddress).Must(IdentityAddress.IsValid);
    }
}
