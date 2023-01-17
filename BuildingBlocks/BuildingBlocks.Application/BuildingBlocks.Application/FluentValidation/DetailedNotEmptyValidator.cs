using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;

namespace Enmeshed.BuildingBlocks.Application.FluentValidation
{
    public static class DetailedNotEmptyValidatorRuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> DetailedNotEmpty<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .DetailedNotNull() // apply this one first in order to get a specific error code for null values
                .NotEmpty()
                .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        }
    }
}