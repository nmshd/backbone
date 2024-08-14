using System.Collections.Concurrent;
using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using FluentValidation;

namespace Backbone.BuildingBlocks.Application.Extensions;

public static class ValidatorExtensions
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> IS_VALID_CACHE = new();

    public static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
        params TProperty[] validOptions)
    {
        return ruleBuilder
            .SetValidator(new ValueInValidator<T, TProperty>(validOptions))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }

    public static IRuleBuilderOptions<T, string?> ValidId<T, TId>(this IRuleBuilder<T, string?> ruleBuilder) where TId : StronglyTypedId
    {
        var method = IS_VALID_CACHE[typeof(TId)];
        if (method == null)
        {
            method = typeof(TId).GetMethod("IsValid", BindingFlags.Public | BindingFlags.Static)!;
            IS_VALID_CACHE.TryAdd(typeof(TId), method);
        }

        return ruleBuilder
            .Must(x => (bool)method.Invoke(null, [x])!)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("The ID is not valid. Check length, prefix and the used characters.");
    }
}
