using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;
using static Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Extensions.IEnumerableExtensions;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.ConsistencyCheck;
public class ConsistencyCheck
{
    private readonly IDataSource _dataSource;
    private readonly IReporter _reporter;

    public ConsistencyCheck(IDataSource dataSource, IReporter reporter)
    {
        _dataSource = dataSource;
        _reporter = reporter;
    }

    /// <summary>
    /// Checks that for any given Identity i, associated with a Tier t, which has several TierQuotaDefinitions tqd, the Identity i has matching tierQuotas tq.
    /// ∀i ∃t : i ∈ t ∧ ∀t.tqd ∃i.tq : tq.DefinitionId = tqd.Address
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Run_for_TierQuotaDefinitions_vs_TierQuotas(CancellationToken cancellationToken)
    {
        var tierQuotaDefinitionIds = await _dataSource.GetTierQuotaDefinitionIds(cancellationToken);
        var tierQuotas = await _dataSource.GetTierQuotasWithDefinitionIds(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return;

        var orphanedTierQuotaIDs = tierQuotas.Where(it => !tierQuotaDefinitionIds.Contains(it.Value)).Select(it => it.Key);

        foreach (var orphanedTierQuotaId in orphanedTierQuotaIDs)
        {
            _reporter.ReportOrphanedTierQuotaId(orphanedTierQuotaId);
        }
    }

    public async Task Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken cancellationToken)
    {
        var identitiesInDevices = await _dataSource.GetDevicesIdentitiesAddresses(cancellationToken);
        if (cancellationToken.IsCancellationRequested) return;

        var identitiesInQuotas = await _dataSource.GetQuotasIdentitiesAddresses(cancellationToken);
        if (cancellationToken.IsCancellationRequested) return;

        var (_, identitiesMissingFromQuotas, identitiesMissingFromDevices) = Distribute(identitiesInDevices, identitiesInQuotas);

        foreach (var i in identitiesMissingFromQuotas)
        {
            _reporter.ReportIdentityMissingFromQuotas(i);
        }

        foreach (var i in identitiesMissingFromDevices)
        {
            _reporter.ReportIdentityMissingFromDevices(i);
        }
    }

    public async Task Run_for_DevicesTiers_vs_QuotasTiers(CancellationToken cancellationToken)
    {
        var tiersInDevices = await _dataSource.GetDevicesTiersIds(cancellationToken);
        if (cancellationToken.IsCancellationRequested) return;

        var tiersInQuotas = await _dataSource.GetQuotasTiersIds(cancellationToken);
        if (cancellationToken.IsCancellationRequested) return;

        var (_, tiersMissingFromQuotas, tiersMissingFromDevices) = Distribute(tiersInDevices, tiersInQuotas);

        foreach (var i in tiersMissingFromQuotas)
        {
            _reporter.ReportTierMissingFromQuotas(i);
        }

        foreach (var i in tiersMissingFromDevices)
        {
            _reporter.ReportTierMissingFromDevices(i);
        }
    }
}
