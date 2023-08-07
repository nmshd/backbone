namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

public class LogReporter : IReporter
{
    private readonly ILogger<LogReporter> _logger;

    private readonly ICollection<string> _tierQuotaDefinitionsMissingFromIdentity;
    private readonly ICollection<string> _tierQuotasMissingFromTier;

    private readonly ICollection<string> _identitiesMissingFromQuotas;
    private readonly ICollection<string> _identitiesMissingFromDevices;

    private readonly ICollection<string> _tiersMissingFromQuotas;
    private readonly ICollection<string> _tiersMissingFromDevices;

    public LogReporter(ILogger<LogReporter> logger)
    {
        _logger = logger;

        _tierQuotaDefinitionsMissingFromIdentity = new List<string>();
        _tierQuotasMissingFromTier = new List<string>();

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

        foreach (var tierQuotaId in _tierQuotaDefinitionsMissingFromIdentity)
        {
            _logger.LogError("no TierQuotaDefinition found for TierQuota with id: {tierQuotaId}.", tierQuotaId);
        }

        foreach (var tierQuotaDefinitionId in _tierQuotasMissingFromTier)
        {
            _logger.LogError("no TierQuota found for TierQuotaDefinition with id: {tierQuotaDefinitionId}.", tierQuotaDefinitionId);
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

    public void ReportTierQuotaDefinitionMissingFromIdentity(string id)
    {
        _tierQuotaDefinitionsMissingFromIdentity.Add(id);
    }

    public void ReportTierQuotaMissingFromTier(string id)
    {
        _tierQuotasMissingFromTier.Add(id);
    }
}
