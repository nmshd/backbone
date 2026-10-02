using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplatesForIdentity;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress)
            .ValidId<Command, IdentityAddress>();
    }
}
