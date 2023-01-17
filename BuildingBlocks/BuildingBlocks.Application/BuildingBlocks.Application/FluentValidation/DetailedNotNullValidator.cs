using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;

namespace Enmeshed.BuildingBlocks.Application.FluentValidation
{
    public static class DetailedNotNullValidatorRuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> DetailedNotNull<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .NotNull()
                .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        }
    }
}