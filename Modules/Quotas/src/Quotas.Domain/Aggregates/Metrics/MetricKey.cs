using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

public record MetricKey
{
    public static readonly MetricKey NUMBER_OF_SENT_MESSAGES = new("NumberOfSentMessages");
    public static readonly MetricKey NUMBER_OF_RELATIONSHIPS = new("NumberOfRelationships");
    public static readonly MetricKey NUMBER_OF_RELATIONSHIP_TEMPLATES = new("NumberOfRelationshipTemplates");
    public static readonly MetricKey NUMBER_OF_FILES = new("NumberOfFiles");
    public static readonly MetricKey NUMBER_OF_TOKENS = new("NumberOfTokens");
    public static readonly MetricKey USED_FILE_STORAGE_SPACE = new("UsedFileStorageSpace");
    public static readonly MetricKey NUMBER_OF_STARTED_DELETION_PROCESSES = new("NumberOfStartedDeletionProcesses");
    public static readonly MetricKey NUMBER_OF_CREATED_DATAWALLET_MODIFICATIONS = new("NumberOfCreatedDatawalletModifications");
    public static readonly MetricKey NUMBER_OF_CREATED_DEVICES = new("NumberOfCreatedDevices");
    public static readonly MetricKey NUMBER_OF_CREATED_CHALLENGES = new("NumberOfCreatedChallenges");

    private static readonly MetricKey[] SUPPORTED_METRIC_KEYS =
    [
        NUMBER_OF_SENT_MESSAGES,
        NUMBER_OF_RELATIONSHIPS,
        NUMBER_OF_FILES,
        NUMBER_OF_TOKENS,
        USED_FILE_STORAGE_SPACE,
        NUMBER_OF_RELATIONSHIP_TEMPLATES,
        NUMBER_OF_STARTED_DELETION_PROCESSES,
        NUMBER_OF_CREATED_DATAWALLET_MODIFICATIONS,
        NUMBER_OF_CREATED_DEVICES,
        NUMBER_OF_CREATED_CHALLENGES
    ];

    private static readonly string[] SUPPORTED_METRIC_KEY_VALUES = SUPPORTED_METRIC_KEYS.Select(m => m.Value).ToArray();

    private MetricKey(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<MetricKey, DomainError> Parse(string value)
    {
        if (!SUPPORTED_METRIC_KEY_VALUES.Contains(value))
            return Result.Failure<MetricKey, DomainError>(DomainErrors.UnsupportedMetricKey());

        return Result.Success<MetricKey, DomainError>(new MetricKey(value));
    }

    public static MetricKey[] GetSupportedMetricKeys()
    {
        return SUPPORTED_METRIC_KEYS;
    }
}
