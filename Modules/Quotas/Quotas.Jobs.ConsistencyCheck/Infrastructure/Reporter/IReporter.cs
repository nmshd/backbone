namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public interface IReporter
{
    void Complete();

    void ReportOrphanedTierQuotaId(string id);
    void ReportOrphanedIdentityIdOnDevices(string orphanedIdentityId);
    void ReportOrphanedIdentityIdOnQuotas(string orphanedIdentityId);
}
