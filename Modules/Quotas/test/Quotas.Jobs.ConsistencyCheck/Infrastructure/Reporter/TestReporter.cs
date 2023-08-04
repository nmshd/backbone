using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.Reporter;
public class TestReporter : IReporter
{
    public List<string> ReportedOrphanedIdentityIdOnDevices { get; } = new();
    public List<string> ReportedOrphanedIdentityIdOnQuotas { get; } = new();
    public List<string> ReportedOrphanedTierIdOnDevices { get; } = new();
    public List<string> ReportedOrphanedTierIdOnQuotas { get; } = new();

    public void Complete()
    {
        throw new NotImplementedException();
    }

    public void ReportOrphanedIdentityIdOnDevices(string orphanedIdentityId)
    {
        ReportedOrphanedIdentityIdOnDevices.Add(orphanedIdentityId);
    }

    public void ReportOrphanedIdentityIdOnQuotas(string orphanedIdentityId)
    {
        ReportedOrphanedIdentityIdOnQuotas.Add(orphanedIdentityId);
    }

    public void ReportOrphanedTierIdOnDevices(string orphanedIdentityId)
    {
        ReportedOrphanedTierIdOnDevices.Add(orphanedIdentityId);
    }

    public void ReportOrphanedTierIdOnQuotas(string orphanedIdentityId)
    {
        ReportedOrphanedTierIdOnQuotas.Add(orphanedIdentityId);
    }

    public void ReportOrphanedTierQuotaId(string id)
    {
        throw new NotImplementedException();
    }
}
