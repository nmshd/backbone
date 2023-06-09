namespace Backbone.Modules.Quotas.Domain;
public interface IMetricCalculator
{
    public uint CalculateUsage(DateTime from, DateTime to, string identityAddress);
}
