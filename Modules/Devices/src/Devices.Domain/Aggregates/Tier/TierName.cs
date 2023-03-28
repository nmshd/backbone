using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public record TierName
{
    public string Value { get; }

    private TierName(string value)
    {
        Value = value;
    }

    public static Result<TierName, DomainError> Create(string value)
    {
        var validationResult = Validate(value);
        if (validationResult != null)
            return Result.Failure<TierName, DomainError>(validationResult);
        return Result.Success<TierName, DomainError>(new TierName(value));
    }

    public static DomainError? Validate(string value)
    {
        if (value.Length > 30)
            return DomainErrors.InvalidTierName("Tier Name is longer than the 30 characters");

        return null;
    }
}
