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
        var identities = await _dataSource.GetIdentities(cancellationToken);
        var tiers = await _dataSource.GetTiers(cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;

        foreach (var identity in identities)
        {
            var tier = tiers.Single(t => t.Id == identity.TierId);
            var tierQuotaDefinitions = tier.Quotas;

            var (_,
                tqdsMissingFromIdentity,
                tqsMissingFromTier
                ) = Distribute(
                tierQuotaDefinitions.Select(x => x.Id),
                identity.TierQuotas.Select(x => x.DefinitionId)
                );

            foreach (var id in tqdsMissingFromIdentity)
            {
                _reporter.ReportTierQuotaDefinitionMissingFromIdentity(id);
            }

            foreach (var id in tqsMissingFromTier)
            {
                _reporter.ReportTierQuotaMissingFromTier(id);
            }
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
