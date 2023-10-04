using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Domain;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;

    private readonly ICollection<IdentityAddressTierQuotaDefinitionIdPair> _tierQuotasMissingFromIdentity;

    private readonly ICollection<string> _identitiesMissingFromQuotas;
    private readonly ICollection<string> _identitiesMissingFromDevices;

    private readonly ICollection<string> _tiersMissingFromQuotas;
    private readonly ICollection<string> _tiersMissingFromDevices;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _tierQuotasMissingFromIdentity = new List<IdentityAddressTierQuotaDefinitionIdPair>();

        _identitiesMissingFromDevices = new List<string>();
        _identitiesMissingFromQuotas = new List<string>();

        _tiersMissingFromQuotas = new List<string>();
        _tiersMissingFromDevices = new List<string>();
    }

    public void Complete()
    {
        foreach (var identityAddress in _identitiesMissingFromQuotas)
        {
            _logger.LogError("Identity with address {identityAddress} found on Devices but missing from Quotas.", identityAddress);
        }

        foreach (var identityAddress in _identitiesMissingFromDevices)
        {
            _logger.LogError("Identity with address {identityAddress} found on Quotas but missing from Devices.", identityAddress);
        }

        foreach (var id in _tiersMissingFromDevices)
        {
            _logger.LogError("Tier with id {id} found on Quotas but missing from Devices.", id);
        }

        foreach (var id in _tiersMissingFromQuotas)
        {
            _logger.LogError("Tier with id {id} found on Devices but missing from Quotas.", id);
        }

        foreach (var pair in _tierQuotasMissingFromIdentity)
        {
            _logger.LogError("no TierQuota found for TierQuotaDefinition with id: {tierQuotaDefinitionId} and Identity with Address {Address}.", pair.TierQuotaDefinitionId, pair.IdentityAddress);
        }
    }

    public void ReportIdentityMissingFromQuotas(string address)
    {
        _identitiesMissingFromQuotas.Add(address);
    }

    public void ReportIdentityMissingFromDevices(string address)
    {
        _identitiesMissingFromDevices.Add(address);
    }

    public void ReportTierMissingFromQuotas(string orphanedIdentityId)
    {
        _tiersMissingFromQuotas.Add(orphanedIdentityId);
    }

    public void ReportTierMissingFromDevices(string orphanedIdentityId)
    {
        _tiersMissingFromDevices.Add(orphanedIdentityId);
    }

    public void ReportTierQuotaDefinitionMissingFromIdentity(IdentityAddressTierQuotaDefinitionIdPair identityAddressTierQuotaDefinitionIdPair)
    {
        _tierQuotasMissingFromIdentity.Add(identityAddressTierQuotaDefinitionIdPair);
    }
}
