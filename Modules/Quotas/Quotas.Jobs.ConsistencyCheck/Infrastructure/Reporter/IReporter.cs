using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Domain;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public interface IReporter
{
    void Complete();

    void ReportTierQuotaDefinitionMissingFromIdentity(IdentityAddressTierQuotaDefinitionIdPair identityAddressTierQuotaDefinitionIdPair);

    void ReportIdentityMissingFromQuotas(string address);
    void ReportIdentityMissingFromDevices(string address);

    void ReportTierMissingFromQuotas(string orphanedIdentityId);
    void ReportTierMissingFromDevices(string orphanedIdentityId);
}
