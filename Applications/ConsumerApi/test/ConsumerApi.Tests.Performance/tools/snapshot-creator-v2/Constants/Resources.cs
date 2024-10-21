namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Constants;

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

    public const string INVALID_FILE_PATH = "Invalid file path";
    public const string POOL_CONFIG_JSON_NAME = "pool-config";
    public const string POOL_CONFIG_JSON_EXT = "json";

    public const string WORKBOOK_SHEET_TEST_LOAD = "test";
    public const string WORKBOOK_SHEET_LIGHT_LOAD = "light";
    public const string WORKBOOK_SHEET_MEDIUM_LOAD = "medium";
    public const string WORKBOOK_SHEET_HEAVY_LOAD = "heavy";

    public const string RELATIONSHIPS_AND_MESSAGE_POOL_CONFIGS_FILE_NAME = "RelationshipsAndMessagePoolConfigs";
    public const string EXCEL_FILE_EXT = "xlsx";

    public const string APP_TOTAL_NUMBER_OF_SENT_MESSAGES = "App.TotalNumberOfSentMessages";
    public const string APP_TOTAL_NUMBER_OF_RECEIVED_MESSAGES = "App.TotalNumberOfReceivedMessages";
    public const string APP_NUMBER_OF_RECEIVED_MESSAGES_ADD_ON = "App.NumberOfReceivedMessagesAddOn";
    public const string APP_TOTAL_NUMBER_OF_RELATIONSHIPS = "App.TotalNumberOfRelationships";

    public const string CONNECTOR_TOTAL_NUMBER_OF_SENT_MESSAGES = "Connector.TotalNumberOfSentMessages";
    public const string CONNECTOR_TOTAL_NUMBER_OF_RECEIVED_MESSAGES = "Connector.TotalNumberOfReceivedMessages";
    public const string CONNECTOR_NUMBER_OF_RECEIVED_MESSAGES_ADD_ON = "Connector.NumberOfReceivedMessagesAddOn";
    public const string CONNECTOR_TOTAL_NUMBER_OF_AVAILABLE_RELATIONSHIPS = "Connector.TotalNumberOfAvailableRelationships";

    public const string CONNECTOR_NO_MORE_IDENTITIES_AVAILABLE = "No more connector identities available";

    public const string IDENTITY_NO_MORE_RELATIONSHIPS_AVAILABLE = "No more relationships available";
}
