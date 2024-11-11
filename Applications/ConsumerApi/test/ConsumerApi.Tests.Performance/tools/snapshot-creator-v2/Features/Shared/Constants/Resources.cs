namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Constants;

public static class Resources
{
    public const string POOL_TYPE_NEVER = "never";
    public const string POOL_TYPE_APP = "app";
    public const string POOL_TYPE_CONNECTOR = "connector";
    public const string POOL_TYPE_UNKNOWN = "Invalid identity pool type";

    public const string POOL_NAME_NEVER = "NeverUse";
    public const string POOL_NAME_APP_LIGHT = "AppLight";
    public const string POOL_NAME_APP_MEDIUM = "AppMedium";
    public const string POOL_NAME_APP_HEAVY = "AppHeavy";
    public const string POOL_NAME_CONNECTOR_LIGHT = "ConnectorLight";
    public const string POOL_NAME_CONNECTOR_MEDIUM = "ConnectorMedium";
    public const string POOL_NAME_CONNECTOR_HEAVY = "ConnectorHeavy";

    public const string POOL_ALIAS_NEVER = "e";
    public const string POOL_ALIAS_APP_LIGHT = "a1";
    public const string POOL_ALIAS_APP_MEDIUM = "a2";
    public const string POOL_ALIAS_APP_HEAVY = "a3";
    public const string POOL_ALIAS_CONNECTOR_LIGHT = "c1";
    public const string POOL_ALIAS_CONNECTOR_MEDIUM = "c2";
    public const string POOL_ALIAS_CONNECTOR_HEAVY = "c3";

    public const string PERFORMANCE_TEST_CONFIGURATION_EXCEL_FILE_EMPTY = "Excel file is empty";
    public const string PERFORMANCE_TEST_CONFIGURATION_FIRST_ROW_MISMATCH = "First row is not of type";

    public const string POOL_CONFIG_JSON_WITH_RELATIONSHIP_AND_MESSAGES = "pool-config-relationships";
    public const string JSON_FILE_EXT = "json";

    public const string WORKBOOK_SHEET_TEST_LOAD = "test";
    public const string WORKBOOK_SHEET_LIGHT_LOAD = "light";
    public const string WORKBOOK_SHEET_HEAVY_LOAD = "heavy";

    public const string TOTAL_NUMBER_OF_RELATIONSHIPS = "App.TotalNumberOfRelationships";
    public const string APP_TOTAL_NUMBER_OF_SENT_MESSAGES = "App.TotalNumberOfSentMessages";
    public const string CONNECTOR_TOTAL_NUMBER_OF_SENT_MESSAGES = "Connector.TotalNumberOfSentMessages";

    public const string IDENTITY_NO_MORE_RELATIONSHIPS_AVAILABLE = "No more relationships available";

    public const string RELATIONSHIP_NO_RECIPIENT_AVAILABLE = "No further recipient identity available to establish a relationship to sender identity Address: {0} of {1}";
    public const string RELATIONSHIP_COUNT_MISMATCH = "Relationship count mismatch. Expected: {0}, Actual: {1}";

    public const string VERIFICATION_TOTAL_NUMBER_OF_SENT_MESSAGES_FAILED = "Verification of total number of sent messages to recipient pool {0} failed. Expected: {1}, actual: {2}";

    public const string PERFORMANCE_TEST_CONFIG_READER_INVALID_FILE_EXT = "Invalid file extension. Supported extensions are {0}, actual: {1}";
    public const string IDENTITY_POOL_CONFIGURATION_NOT_CREATED = "Identity Pool Configuration not created, but is a pre-condition to generate relationships";
}
