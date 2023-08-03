using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.Reporter;
public class TestReporter : IReporter
{
    public List<string> ReportedOrphanedIdentityIdOnDevices { get; } = new();
    public List<string> ReportedOrphanedIdentityIdOnQuotas { get; } = new();

    public void Complete()
    {
        throw new NotImplementedException();
    }

    public void ReportOrphanedIdentityIdOnDevices(string orphanedIdentityId)
    {
        throw new NotImplementedException();
    }

    public void ReportOrphanedIdentityIdOnQuotas(string orphanedIdentityId)
    {
        throw new NotImplementedException();
    }

    public void ReportOrphanedTierQuotaId(string id)
    {
        throw new NotImplementedException();
    }
}
