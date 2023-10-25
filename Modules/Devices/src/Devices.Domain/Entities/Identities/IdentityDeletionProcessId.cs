using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.Errors;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record IdentityDeletionProcessId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "IDP";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private IdentityDeletionProcessId(string value) : base(value) { }

    public static IdentityDeletionProcessId Generate()
    {
        var randomPart = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new IdentityDeletionProcessId(PREFIX + randomPart);
    }

    public static Result<IdentityDeletionProcessId, DomainError> Create(string value)
    {
        var validationError = UTILS.Validate(value);

        if (validationError != null)
            return Result.Failure<IdentityDeletionProcessId, DomainError>(validationError);

        return Result.Success<IdentityDeletionProcessId, DomainError>(new IdentityDeletionProcessId(value));
    }
}
