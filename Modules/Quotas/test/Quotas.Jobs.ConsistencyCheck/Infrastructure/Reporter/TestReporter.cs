using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.Reporter;
public class TestReporter : IReporter
{
    public List<string> ReportedIdentitiesMissingFromQuotas { get; } = new();
    public List<string> ReportedIdentitiesMissingFromDevices { get; } = new();
    public List<string> ReportedTiersMissingFromQuotas { get; } = new();
    public List<string> ReportedTiersMissingFromDevices { get; } = new();

    public void Complete()
    {
        throw new NotImplementedException();
    }

    public void ReportIdentityMissingFromQuotas(string orphanedIdentityId)
    {
        ReportedIdentitiesMissingFromQuotas.Add(orphanedIdentityId);
    }

    public void ReportIdentityMissingFromDevices(string orphanedIdentityId)
    {
        ReportedIdentitiesMissingFromDevices.Add(orphanedIdentityId);
    }

    public void ReportTierMissingFromQuotas(string orphanedIdentityId)
    {
        ReportedTiersMissingFromQuotas.Add(orphanedIdentityId);
    }

    public void ReportTierMissingFromDevices(string orphanedIdentityId)
    {
        ReportedTiersMissingFromDevices.Add(orphanedIdentityId);
    }

    public void ReportOrphanedTierQuotaId(string id)
    {
        throw new NotImplementedException();
    }
}
