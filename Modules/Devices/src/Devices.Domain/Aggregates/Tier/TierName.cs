using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public record TierName
{
    public static readonly TierName BASIC_DEFAULT_NAME = new("Basic");

    public string Value { get; }
    public const int MIN_LENGTH = 3;
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
        if (value.Length > MAX_LENGTH || value.Length < MIN_LENGTH)
            return DomainErrors.InvalidTierName($"Tier Name length must be between {MIN_LENGTH} and {MAX_LENGTH}");

        return null;
    }

    public static implicit operator string(TierName name)
    {
        return name.Value;
    }
}
