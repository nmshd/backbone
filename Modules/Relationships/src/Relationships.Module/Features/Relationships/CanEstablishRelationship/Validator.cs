using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;

// ReSharper disable once UnusedType.Global
public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(q => q.PeerAddress).Must(IdentityAddress.IsValid);
    }
}
