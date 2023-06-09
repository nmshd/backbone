using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Domain;
public interface IMetricCalculator
{
    public void CalculateUsage(DateTime from, DateTime to, IdentityAddress identityAddress);
}
