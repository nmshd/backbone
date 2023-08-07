namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public interface IReporter
{
    void Complete();

    void ReportOrphanedTierQuotaId(string id);
    void ReportIdentityMissingFromQuotas(string orphanedIdentityId);
    void ReportIdentityMissingFromDevices(string orphanedIdentityId);

    void ReportTierMissingFromQuotas(string orphanedIdentityId);
    void ReportTierMissingFromDevices(string orphanedIdentityId);
}
