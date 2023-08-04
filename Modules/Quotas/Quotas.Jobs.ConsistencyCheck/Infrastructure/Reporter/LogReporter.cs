namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;
    private readonly ICollection<string> _tierQuotaIds;

    private readonly ICollection<string> _orphanedIdentityIdsOnDevices;
    private readonly ICollection<string> _orphanedIdentityIdsOnQuotas;

    private readonly ICollection<string> _orphanedTierIdsOnDevices;
    private readonly ICollection<string> _orphanedTierIdsOnQuotas;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;
        _tierQuotaIds = new List<string>();
        _orphanedIdentityIdsOnQuotas = new List<string>();
        _orphanedIdentityIdsOnDevices = new List<string>();
    }

    public void Complete()
    {
        foreach (var tierQuotaId in _tierQuotaIds)
        {
            _logger.LogError("no TierQuotaDefinition found for TierQuota with id: {tierQuotaId}.", tierQuotaId);
        }

        foreach (var identityId in _orphanedIdentityIdsOnDevices)
        {
            _logger.LogError("Identity with id {identityId} found on Devices but missing from Quotas.", identityId);
        }

        foreach (var identityId in _orphanedIdentityIdsOnQuotas)
        {
            _logger.LogError("Identity with id {identityId} found on Quotas but missing from Devices.", identityId);
        }
    }

    public void ReportOrphanedIdentityIdOnDevices(string id)
    {
        _orphanedIdentityIdsOnDevices.Add(id);
    }

    public void ReportOrphanedIdentityIdOnQuotas(string id)
    {
        _orphanedIdentityIdsOnQuotas.Add(id);
    }

    public void ReportOrphanedTierIdOnDevices(string orphanedIdentityId)
    {
        _orphanedTierIdsOnDevices.Add(orphanedIdentityId);
    }

    public void ReportOrphanedTierIdOnQuotas(string orphanedIdentityId)
    {
        _orphanedTierIdsOnQuotas.Add(orphanedIdentityId);
    }

    public void ReportOrphanedTierQuotaId(string id)
    {
        _tierQuotaIds.Add(id);
    }
}
