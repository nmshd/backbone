using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<Command, IdentityAddress>();
    }
}
