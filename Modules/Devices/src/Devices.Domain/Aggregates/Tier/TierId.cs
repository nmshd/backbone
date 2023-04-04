using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.Errors;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public record TierId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "TIR";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private TierId(string value) : base(value) { }

    public static TierId Generate()
    {
        var value = PREFIX + StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new TierId(value);
    }

    public static Result<TierId, DomainError> Create(string value)
    {
        var validationResult = UTILS.Validate(value);
        if (validationResult != null)
            return Result.Failure<TierId, DomainError>(validationResult);
        return Result.Success<TierId, DomainError>(new TierId(value));
    }
}
