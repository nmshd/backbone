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
}
