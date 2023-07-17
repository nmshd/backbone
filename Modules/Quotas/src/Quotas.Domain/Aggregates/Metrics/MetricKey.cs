using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

public record MetricKey
{
    public static MetricKey NumberOfSentMessages = new("NumberOfSentMessages");
    public static MetricKey NumberOfRelationships = new("NumberOfRelationships");
    public static MetricKey NumberOfFiles = new("NumberOfFiles");
    public static MetricKey FileStorageCapacity = new("FileStorageCapacity");

    private static readonly MetricKey[] SUPPORTED_METRIC_KEYS = {
        NumberOfSentMessages,
        NumberOfRelationships,
        NumberOfFiles,
        FileStorageCapacity
    };
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

    public static string[] GetSupportedMetricKeyValues()
    {
        return SUPPORTED_METRIC_KEY_VALUES;
    }
}
