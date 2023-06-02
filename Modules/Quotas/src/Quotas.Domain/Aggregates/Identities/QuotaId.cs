using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.Errors;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public record QuotaId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "QUO";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private QuotaId(string value) : base(value)
    {

    }

    public static QuotaId Generate()
    {
        var value = PREFIX + StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new QuotaId(value);
    }

    public static Result<QuotaId, DomainError> Create(string value)
    {
        var validationResult = UTILS.Validate(value);
        if (validationResult != null)
            return Result.Failure<QuotaId, DomainError>(validationResult);
        return Result.Success<QuotaId, DomainError>(new QuotaId(value));
    }

    public static QuotaId New()
    {
        var randomPart = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new QuotaId(PREFIX + randomPart);
    }
}
