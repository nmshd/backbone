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
    /// ∀i ∃t : i ∈ t ∧ ∀t.tqd ∃i.tq : tq.DefinitionId = tqd.Id
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Run_for_TierQuotaDefinitions_vs_TierQuotas(CancellationToken cancellationToken)
    {
        var tierQuotaDefinitionIds = await _dataSource.GetTierQuotaDefinitionIds();
        var tierQuotas = await _dataSource.GetTierQuotasWithDefinitionIds();

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
        var devicesIdentitiesIds = await _dataSource.GetDevicesIdentitiesIds();
        if (cancellationToken.IsCancellationRequested) return;

        var quotasIdentitiesIds = await _dataSource.GetQuotasIdentitiesIds();
        if (cancellationToken.IsCancellationRequested) return;

        var (_, orphanedOnDevices, orphanedOnQuotas) = Distribute(devicesIdentitiesIds, quotasIdentitiesIds);

        foreach (var orphanedIdentityId in orphanedOnDevices)
        {
            _reporter.ReportOrphanedIdentityIdOnDevices(orphanedIdentityId);
        }

        foreach (var orphanedIdentityId in orphanedOnQuotas)
        {
            _reporter.ReportOrphanedIdentityIdOnQuotas(orphanedIdentityId);
        }
    }

    public async Task Run_for_DevicesTiers_vs_QuotasTiers(CancellationToken cancellationToken)
    {
        var devicesTiersIds = await _dataSource.GetDevicesTiersIds();
        if (cancellationToken.IsCancellationRequested) return;

        var quotasTiersIds = await _dataSource.GetQuotasTiersIds();
        if (cancellationToken.IsCancellationRequested) return;

        var (_, orphanedOnDevices, orphanedOnQuotas) = Distribute(devicesTiersIds, quotasTiersIds);

        foreach (var orphanedIdentityId in orphanedOnDevices)
        {
            _reporter.ReportOrphanedTierIdOnDevices(orphanedIdentityId);
        }

        foreach (var orphanedIdentityId in orphanedOnQuotas)
        {
            _reporter.ReportOrphanedTierIdOnQuotas(orphanedIdentityId);
        }
    }
}
