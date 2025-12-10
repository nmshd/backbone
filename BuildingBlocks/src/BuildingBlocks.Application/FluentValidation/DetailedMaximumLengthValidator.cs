using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;

namespace Backbone.BuildingBlocks.Application.FluentValidation;

public static class DetailedMaximumLengthValidatorRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string?> DetailedMaximumLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int maximumLength)
    {
        return ruleBuilder
            .MaximumLength(maximumLength)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
