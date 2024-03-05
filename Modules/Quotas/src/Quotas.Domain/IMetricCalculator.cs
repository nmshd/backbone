namespace Backbone.Modules.Quotas.Domain;
public interface IMetricCalculator
{
    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken);
}
