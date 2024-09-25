using System.Collections.Concurrent;
using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Extensions;
public static class ValidatorExtensions
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> IS_VALID_CACHE = new();

    public static IRuleBuilderOptions<T, string?> ValidCommunicationLanguage<T, TLanguage>(this IRuleBuilder<T, string?> ruleBuilder) where TLanguage : CommunicationLanguage
    {
        if (!IS_VALID_CACHE.TryGetValue(typeof(TLanguage), out var method))
        {
            method = typeof(TLanguage).GetMethod("IsValid", BindingFlags.Public | BindingFlags.Static) ??
                     throw new Exception($"Type '{typeof(TLanguage)}' does not implement required 'IsValid' method.");
            IS_VALID_CACHE.TryAdd(typeof(TLanguage), method);
        }

        return ruleBuilder
            .Must(x => (bool)method.Invoke(null, [x])!)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("The device language is not valid. Check length and used characters.");
    }
}
