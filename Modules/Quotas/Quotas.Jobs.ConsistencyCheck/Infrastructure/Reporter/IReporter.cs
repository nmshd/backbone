namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public interface IReporter
{
    void Complete();

    void ReportOrphanedTierQuotaId(string id);
    void ReportOrphanedIdentityIdOnDevices(string orphanedIdentityId);
    void ReportOrphanedIdentityIdOnQuotas(string orphanedIdentityId);

    void ReportOrphanedTierIdOnDevices(string orphanedIdentityId);
    void ReportOrphanedTierIdOnQuotas(string orphanedIdentityId);
}
