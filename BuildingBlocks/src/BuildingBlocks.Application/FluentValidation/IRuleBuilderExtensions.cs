using Enmeshed.BuildingBlocks.Domain.Errors;
using FluentValidation;
using FluentValidation.Results;

namespace Enmeshed.BuildingBlocks.Application.FluentValidation;

public static class IRuleBuilderExtensions
{
    public static void Valid<T>(this IRuleBuilder<T, string> ruleBuilder,
        Func<string, DomainError?> validator)
    {
        ruleBuilder.Custom((v, context) =>
        {
            var domainError = validator(v);
            if (domainError != null)
                context.AddFailure(new ValidationFailure(context.PropertyName, domainError.Message, v) { ErrorCode = domainError.Code });
        });
    }
}
