using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Extensions;
public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidCommunicationLanguage<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => x != null && CommunicationLanguage.IsValid(x))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("The device language is not valid. Check length and used characters.");
    }
}
