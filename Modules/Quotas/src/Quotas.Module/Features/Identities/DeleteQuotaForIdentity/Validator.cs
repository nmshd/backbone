using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using FluentValidation;

namespace Backbone.Modules.Quotas.Module.Features.Identities.DeleteQuotaForIdentity;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress).ValidId<Command, IdentityAddress>();
        RuleFor(c => c.IndividualQuotaId).ValidId<Command, QuotaId>();
    }
}
