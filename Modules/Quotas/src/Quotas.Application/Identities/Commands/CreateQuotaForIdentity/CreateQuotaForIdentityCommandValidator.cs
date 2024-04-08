using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;

public class CreateQuotaForIdentityCommandValidator : AbstractValidator<CreateQuotaForIdentityCommand>
{
    public CreateQuotaForIdentityCommandValidator()
    {
        RuleFor(c => c.IdentityAddress)
            .DetailedNotEmpty();
        RuleFor(c => c.Max)
            .GreaterThan(0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        RuleFor(c => c.Period)
            .DetailedNotNull();
        RuleFor(c => c.MetricKey)
            .DetailedNotNull();
    }
}
