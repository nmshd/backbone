using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

public record MetricKey
{
    public static MetricKey NumberOfSentMessages = new("NumberOfSentMessages");
    public static MetricKey NumberOfRelationships = new("NumberOfRelationships");
    public static MetricKey NumberOfFiles = new("NumberOfFiles");
    public static MetricKey FileStorageCapacity = new("FileStorageCapacity");

    private static readonly MetricKey[] SupportedMetricKeys = {
        NumberOfSentMessages,
        NumberOfRelationships,
        NumberOfFiles,
        FileStorageCapacity
    };
    private static readonly string[] SupportedMetricKeyValues = SupportedMetricKeys.Select(m => m.Value).ToArray();

    private MetricKey(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<MetricKey, DomainError> Parse(string value)
    {
        if (!SupportedMetricKeyValues.Contains(value))
            return Result.Failure<MetricKey, DomainError>(DomainErrors.UnsupportedMetricKey());

        return Result.Success<MetricKey, DomainError>(new MetricKey(value));
    }

    public static string[] GetSupportedMetricKeyValues()
    {
        return SupportedMetricKeyValues;
    }
}