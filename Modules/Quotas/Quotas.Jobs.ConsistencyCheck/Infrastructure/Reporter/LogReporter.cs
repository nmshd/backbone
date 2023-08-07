namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly ICollection<string> _tierQuotaIds;

    private readonly ICollection<string> _identitiesMissingFromQuotas;
    private readonly ICollection<string> _identititesMissingFromDevices;

    private readonly ICollection<string> _tiersMissingFromQuotas;
    private readonly ICollection<string> _tiersMissingFromDevices;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;
        _tierQuotaIds = new List<string>();
        _identititesMissingFromDevices = new List<string>();
        _identitiesMissingFromQuotas = new List<string>();
    }

    public void Complete()
    {
        foreach (var tierQuotaId in _tierQuotaIds)
        {
            _logger.LogError("no TierQuotaDefinition found for TierQuota with id: {tierQuotaId}.", tierQuotaId);
        }

        foreach (var identityId in _identitiesMissingFromQuotas)
        {
            _logger.LogError("Identity with id {identityId} found on Devices but missing from Quotas.", identityId);
        }

        foreach (var identityId in _identititesMissingFromDevices)
        {
            _logger.LogError("Identity with id {identityId} found on Quotas but missing from Devices.", identityId);
        }
    }

    public void ReportIdentityMissingFromQuotas(string id)
    {
        _identitiesMissingFromQuotas.Add(id);
    }

    public void ReportIdentityMissingFromDevices(string id)
    {
        _identititesMissingFromDevices.Add(id);
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
