using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record IdentityDeletionProcessAuditLogEntryId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "IDA";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private IdentityDeletionProcessAuditLogEntryId(string value) : base(value) { }

    public static IdentityDeletionProcessAuditLogEntryId Generate()
    {
        var randomPart = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new IdentityDeletionProcessAuditLogEntryId(PREFIX + randomPart);
    }

    public static Result<IdentityDeletionProcessAuditLogEntryId, DomainError> Create(string value)
    {
        var validationError = UTILS.Validate(value);

        if (validationError != null)
            return Result.Failure<IdentityDeletionProcessAuditLogEntryId, DomainError>(validationError);

        return Result.Success<IdentityDeletionProcessAuditLogEntryId, DomainError>(new IdentityDeletionProcessAuditLogEntryId(value));
    }
}
