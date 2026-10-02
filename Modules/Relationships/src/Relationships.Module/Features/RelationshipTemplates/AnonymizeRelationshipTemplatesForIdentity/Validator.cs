using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplatesForIdentity;

public class Validator : AbstractValidator<AnonymizeRelationshipTemplatesForIdentityCommand>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress)
            .ValidId<AnonymizeRelationshipTemplatesForIdentityCommand, IdentityAddress>();
    }
}
