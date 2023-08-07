namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly ICollection<string> _tierQuotaIds;

    private readonly ICollection<string> _identitiesMissingFromQuotas;
    private readonly ICollection<string> _identitiesMissingFromDevices;

    private readonly ICollection<string> _tiersMissingFromQuotas;
    private readonly ICollection<string> _tiersMissingFromDevices;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;
        _tierQuotaIds = new List<string>();
        _identitiesMissingFromDevices = new List<string>();
        _identitiesMissingFromQuotas = new List<string>();
    }

    public void Complete()
    {
        foreach (var tierQuotaId in _tierQuotaIds)
        {
            _logger.LogError("no TierQuotaDefinition found for TierQuota with address: {tierQuotaId}.", tierQuotaId);
        }

        foreach (var identityAddress in _identitiesMissingFromQuotas)
        {
            _logger.LogError("Identity with address {identityAddress} found on Devices but missing from Quotas.", identityAddress);
        }

        foreach (var identityAddress in _identitiesMissingFromDevices)
        {
            _logger.LogError("Identity with address {identityAddress} found on Quotas but missing from Devices.", identityAddress);
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

    public void ReportOrphanedTierQuotaId(string id)
    {
        _tierQuotaIds.Add(id);
    }
}
