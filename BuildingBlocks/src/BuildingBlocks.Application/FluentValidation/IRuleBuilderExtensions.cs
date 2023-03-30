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
            var r = validator(v);
            if (r != null)
                context.AddFailure(new ValidationFailure(context.PropertyName, r.Message, v) { ErrorCode = r.Code });
        });
    }
}