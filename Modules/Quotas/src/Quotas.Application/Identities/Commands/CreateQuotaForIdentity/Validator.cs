using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;

public class Validator : AbstractValidator<CreateQuotaForIdentityCommand>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress).ValidId<CreateQuotaForIdentityCommand, IdentityAddress>();
        RuleFor(c => c.Max)
            .GreaterThan(0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        RuleFor(c => c.Period)
            .DetailedNotNull();
        RuleFor(c => c.MetricKey)
            .DetailedNotNull();
    }
}
