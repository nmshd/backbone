using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

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

    public async Task Run_for_TierQuotaDefinitions_vs_TierQuotas(CancellationToken cancellationToken)
    {
        var tierQuotasMissingFromIdentities = await _dataSource.GetTierQuotasMissingFromIdentities(cancellationToken);

        foreach (var identityAddressTierQuotaDefinitionIdPair in tierQuotasMissingFromIdentities)
        {
            _reporter.ReportTierQuotaDefinitionMissingFromIdentity(identityAddressTierQuotaDefinitionIdPair);
        }
    }

    public async Task Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken cancellationToken)
    {
        var identitiesMissingFromQuotas = await _dataSource.GetIdentitiesMissingFromQuotas(cancellationToken);
        var identitiesMissingFromDevices = await _dataSource.GetIdentitiesMissingFromDevices(cancellationToken);

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
        var tiersMissingFromQuotas = await _dataSource.GetTiersMissingFromQuotas(cancellationToken);
        var tiersMissingFromDevices = await _dataSource.GetTiersMissingFromDevices(cancellationToken);


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
