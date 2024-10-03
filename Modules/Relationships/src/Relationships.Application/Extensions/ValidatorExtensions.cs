using System.Collections.Concurrent;
using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Extensions;
public static class ValidatorExtensions
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> IS_VALID_CACHE = new();

    public static IRuleBuilderOptions<T, TQuery?> ValidRelationshipTemplate<T, TQuery, TId>(
        this IRuleBuilder<T, TQuery?> ruleBuilder,
        Func<TQuery, string> idSelector,
        Func<TQuery, byte[]?> passwordSelector
    ) where TId : StronglyTypedId
    {
        if (!IS_VALID_CACHE.TryGetValue(typeof(TId), out var method))
        {
            method = typeof(TId).GetMethod("IsValid", BindingFlags.Public | BindingFlags.Static) ??
                     throw new Exception($"Type '{typeof(TId)}' does not implement required 'IsValid' method.");
            IS_VALID_CACHE.TryAdd(typeof(TId), method);
        }

        return ruleBuilder
            .Must(x => x != null && (bool)method.Invoke(null, [idSelector(x)])!)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("The ID is not valid. Check length, prefix and the used characters.")
            .Must(x => x == null || passwordSelector(x) == null || passwordSelector(x)!.Length > 0)
            .WithMessage("The password, if provided, must be valid.");
    }
}
