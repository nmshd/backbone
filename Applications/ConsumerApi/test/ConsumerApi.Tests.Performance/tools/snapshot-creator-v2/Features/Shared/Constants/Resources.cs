using System.Text;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Constants;

public static class Resources
{
    public const string POOL_TYPE_NEVER = "never";
    public const string POOL_TYPE_APP = "app";
    public const string POOL_TYPE_CONNECTOR = "connector";
    public const string POOL_TYPE_UNKNOWN = "Invalid identity pool type";

    public const string PERFORMANCE_TEST_CONFIGURATION_EXCEL_FILE_EMPTY = "Excel file is empty";
    public const string PERFORMANCE_TEST_CONFIGURATION_FIRST_ROW_MISMATCH = "First row is not of type";

    public const string TOTAL_NUMBER_OF_RELATIONSHIPS = "App.TotalNumberOfRelationships";
    public const string APP_TOTAL_NUMBER_OF_SENT_MESSAGES = "App.TotalNumberOfSentMessages";
    public const string CONNECTOR_TOTAL_NUMBER_OF_SENT_MESSAGES = "Connector.TotalNumberOfSentMessages";

    public const string RELATIONSHIP_COUNT_MISMATCH = "Relationship count mismatch. Expected: {0}, Actual: {1}";

    public const string VERIFICATION_TOTAL_NUMBER_OF_SENT_MESSAGES_FAILED = "Verification of total number of sent messages to recipient pool {0} failed. Expected: {1}, actual: {2}";

    public const string PERFORMANCE_TEST_CONFIG_READER_INVALID_FILE_EXT = "Invalid file extension. Supported extensions are {0}, actual: {1}";
    public const string IDENTITY_POOL_CONFIGURATION_NOT_CREATED = "Identity Pool Configuration not created, but is a pre-condition to generate relationships";

    public const string IDENTITY_LOG_SUFFIX = "[IdentityAddress/ConfigurationIdentityAddress/PoolAlias]";

    public static string BuildErrorDetails<TResult>(string message, DomainIdentity? senderIdentity, DomainIdentity? recipientIdentity, ApiResponse<TResult>? apiResponse = null)
    {
        var sb = new StringBuilder();

        sb.AppendLine(message);

        if (senderIdentity is not null)
        {
            sb.AppendLine($"Identity: {senderIdentity.IdentityAddress}/{senderIdentity.ConfigurationIdentityAddress}/{senderIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}");
        }

        if (recipientIdentity is not null)
        {
            sb.AppendLine($"Recipient Identity: {recipientIdentity.IdentityAddress}/{recipientIdentity.ConfigurationIdentityAddress}/{recipientIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}");
        }

        if (apiResponse is null) return sb.ToString();

        sb.AppendLine($"HTTP Statuscode: {apiResponse.Status}");

        if (apiResponse.Error is null) return sb.ToString();

        sb.AppendLine($"Error Id: {apiResponse.Error.Id}");
        sb.AppendLine($"Error Code: {apiResponse.Error.Code}");
        sb.AppendLine($"Error Message: {apiResponse.Error?.Message}");

        return sb.ToString();
    }

    public static string BuildErrorDetails<TResult>(string message, DomainIdentity? identity, ApiResponse<TResult>? apiResponse) =>
        BuildErrorDetails(message, identity, recipientIdentity: null, apiResponse);

    public static string BuildErrorDetails(string message, DomainIdentity? identity, DomainIdentity? recipientIdentity) =>
        BuildErrorDetails<object>(message, identity, recipientIdentity);

    public static string BuildRelationshipErrorDetails(string message, DomainIdentity? identity, List<RelationshipIdBag>? expectedItems, List<RelationshipIdBag>? actualItems)
    {
        var sb = new StringBuilder();

        sb.AppendLine(message);

        if (identity is not null)
        {
            sb.AppendLine($"Identity: {identity.IdentityAddress}/{identity.ConfigurationIdentityAddress}/{identity.PoolAlias} {IDENTITY_LOG_SUFFIX}");
        }


        if (expectedItems is { Count: > 0 })
        {
            sb.AppendLine($"Expected: {string.Join(", ", expectedItems.Select(c => $"{c.IdentityAddress}/{c.PoolAlias}/{(c.NumberOfSentMessages == default ? "null" : c.NumberOfSentMessages)}"))}");
        }

        if (actualItems is { Count: > 0 })
        {
            sb.AppendLine($"Actual: {string.Join(", ", actualItems.Select(c => $"{c.IdentityAddress}/{c.PoolAlias}/{(c.NumberOfSentMessages == default ? "null" : c.NumberOfSentMessages)}"))}");
        }

        return sb.ToString();
    }
}
