using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentValidation;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.CreateQuotaForTier;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.TierId).ValidId<Command, TierId>();
        RuleFor(c => c.Max)
            .GreaterThan(0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        RuleFor(c => c.Period)
            .DetailedNotNull();
        RuleFor(c => c.MetricKey)
            .DetailedNotNull();
    }
}
