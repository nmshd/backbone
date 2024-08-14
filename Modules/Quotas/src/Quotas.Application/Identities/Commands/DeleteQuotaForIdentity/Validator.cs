using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;

public class Validator : AbstractValidator<DeleteQuotaForIdentityCommand>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress).ValidId<DeleteQuotaForIdentityCommand, IdentityAddress>();
        RuleFor(c => c.IndividualQuotaId).ValidId<DeleteQuotaForIdentityCommand, QuotaId>();
    }
}
