using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public record TierQuotaDefinitionId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "TQD";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private TierQuotaDefinitionId(string value) : base(value)
    {

    }

    public static TierQuotaDefinitionId Generate()
    {
        var value = PREFIX + StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new TierQuotaDefinitionId(value);
    }

    public static Result<TierQuotaDefinitionId, DomainError> Create(string value)
    {
        var validationResult = UTILS.Validate(value);
        if (validationResult != null)
            return Result.Failure<TierQuotaDefinitionId, DomainError>(validationResult);
        return Result.Success<TierQuotaDefinitionId, DomainError>(new TierQuotaDefinitionId(value));
    }

    public static TierQuotaDefinitionId New()
    {
        var randomPart = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new TierQuotaDefinitionId(PREFIX + randomPart);
    }
}
