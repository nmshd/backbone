using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;

namespace Backbone.BuildingBlocks.Application.FluentValidation;

public static class DetailedNullValidatorRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, TProperty> DetailedNull<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder
            .Null()
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
