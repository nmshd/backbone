using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

public record MetricKey
{
    // ReSharper disable InconsistentNaming
    public static readonly MetricKey NumberOfSentMessages = new("NumberOfSentMessages");
    public static readonly MetricKey NumberOfRelationships = new("NumberOfRelationships");
    public static readonly MetricKey NumberOfRelationshipTemplates = new("NumberOfRelationshipTemplates");
    public static readonly MetricKey NumberOfFiles = new("NumberOfFiles");
    public static readonly MetricKey NumberOfTokens = new("NumberOfTokens");
    public static readonly MetricKey UsedFileStorageSpace = new("UsedFileStorageSpace");
    public static readonly MetricKey NumberOfStartedDeletionProcesses = new("NumberOfStartedDeletionProcesses");
    public static readonly MetricKey NumberOfCreatedDatawalletModifications = new("NumberOfCreatedDatawalletModifications");
    public static readonly MetricKey NumberOfCreatedDevices = new("NumberOfCreatedDevices");
    public static readonly MetricKey NumberOfCreatedChallenges = new("NumberOfCreatedChallenges");
    // ReSharper restore InconsistentNaming

    private static readonly MetricKey[] SUPPORTED_METRIC_KEYS =
    [
        NumberOfSentMessages,
        NumberOfRelationships,
        NumberOfFiles,
        NumberOfTokens,
        UsedFileStorageSpace,
        NumberOfRelationshipTemplates,
        NumberOfStartedDeletionProcesses,
        NumberOfCreatedDatawalletModifications,
        NumberOfCreatedDevices,
        NumberOfCreatedChallenges
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
