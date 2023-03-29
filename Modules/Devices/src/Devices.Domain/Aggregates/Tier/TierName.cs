using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public record TierName
{
    public string Value { get; }
    public const int MAX_LENGTH = 30;

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
        if (value.Length > MAX_LENGTH)
            return DomainErrors.InvalidTierName($"Tier Name is longer than the {MAX_LENGTH} characters");

        return null;
    }
}
