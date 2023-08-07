namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public interface IReporter
{
    void Complete();

    void ReportTierQuotaDefinitionMissingFromIdentity(string id);
    void ReportTierQuotaMissingFromTier(string id);

    void ReportIdentityMissingFromQuotas(string address);
    void ReportIdentityMissingFromDevices(string address);

    void ReportTierMissingFromQuotas(string orphanedIdentityId);
    void ReportTierMissingFromDevices(string orphanedIdentityId);
}
