namespace Backbone.Modules.Quotas.Domain;
public interface IMetricCalculator
{
    public Task<uint> CalculateUsageAsync(DateTime from, DateTime to, string identityAddress);
}
