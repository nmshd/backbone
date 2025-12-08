namespace Backbone.Modules.Quotas.Domain;

public interface IMetricCalculator
{
    Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken);
}
