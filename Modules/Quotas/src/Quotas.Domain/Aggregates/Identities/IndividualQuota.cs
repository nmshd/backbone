using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class IndividualQuota : Quota
{
    public IndividualQuota(IdentityAddress applyTo) : base(applyTo)
    {
    }

    public override int Weight => throw new NotImplementedException();

    public override MetricKey MetricKey => throw new NotImplementedException();

    public override int Max => throw new NotImplementedException();

    public override QuotaPeriod Period => throw new NotImplementedException();
}
