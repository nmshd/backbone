using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public record TierId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "TIR";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private TierId(string value) : base(value) { }

    public static TierId Generate()
    {
        var randomPart = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new TierId(PREFIX + randomPart);
    }

    public static Result<TierId, DomainError> Create(string value)
    {
        var validationError = UTILS.Validate(value);

        if (validationError != null)
            return Result.Failure<TierId, DomainError>(validationError);

        return Result.Success<TierId, DomainError>(new TierId(value));
    }
}
