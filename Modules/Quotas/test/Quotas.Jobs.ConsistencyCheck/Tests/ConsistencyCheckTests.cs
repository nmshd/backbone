using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.DataSource;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.Reporter;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests;

public class ConsistencyCheckTests
{
    private readonly ConsistencyCheck.Infrastructure.ConsistencyCheck.ConsistencyCheck _consistencyCheck;
    private readonly FakeDataSource _dataSource;
    private readonly TestReporter _reporter;

    public ConsistencyCheckTests()
    {
        _dataSource = new FakeDataSource();
        _reporter = new TestReporter();
        _consistencyCheck = new(_dataSource, _reporter);
    }

    #region DevicesIdentities_vs_QuotasIdentities

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_NoEntries()
    {
        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedOrphanedIdentityIdOnDevices.Should().BeEmpty();
        _reporter.ReportedOrphanedIdentityIdOnQuotas.Should().BeEmpty();
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_ConsistentEntries()
    {
        var id = "identity-id";
        _dataSource.DevicesIdentitiesIds.Add(id);
        _dataSource.QuotasIdentitiesIds.Add(id);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedOrphanedIdentityIdOnDevices.Should().BeEmpty();
        _reporter.ReportedOrphanedIdentityIdOnQuotas.Should().BeEmpty();
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_MissingFromDevices()
    {
        var id = "identity-id";

        _dataSource.QuotasIdentitiesIds.Add(id);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedOrphanedIdentityIdOnDevices.Should().BeEmpty();
        _reporter.ReportedOrphanedIdentityIdOnQuotas.Should().HaveCount(1);
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_MissingFromQuotas()
    {
        var id = "identity-id";

        _dataSource.DevicesIdentitiesIds.Add(id);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedOrphanedIdentityIdOnQuotas.Should().BeEmpty();
        _reporter.ReportedOrphanedIdentityIdOnDevices.Should().HaveCount(1);
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_MissingFromBothSides()
    {
        var idCommon = "identity-id-common";
        var idOnlyInDevices = "identity-id-devices";
        var idOnlyInQuotas = "identity-id-quotas";

        _dataSource.DevicesIdentitiesIds.Add(idCommon);
        _dataSource.QuotasIdentitiesIds.Add(idCommon);

        _dataSource.DevicesIdentitiesIds.Add(idOnlyInDevices);

        _dataSource.QuotasIdentitiesIds.Add(idOnlyInQuotas);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedOrphanedIdentityIdOnDevices.Single().Should().Be(idOnlyInDevices);
        _reporter.ReportedOrphanedIdentityIdOnQuotas.Single().Should().Be(idOnlyInQuotas);
    }

    #endregion

    #region DevicesTiers_vs_QuotasTiers
    
    [Fact]
    public async void SanityCheck_DevicesTiers_vs_QuotasTiers_NoEntries()
    {
        await _consistencyCheck.Run_for_DevicesTiers_vs_QuotasTiers(CancellationToken.None);

        _reporter.ReportedOrphanedTierIdOnDevices.Should().BeEmpty();
        _reporter.ReportedOrphanedTierIdOnQuotas.Should().BeEmpty();
    }

    [Fact]
    public async void SanityCheck_DevicesTiers_vs_QuotasTiers_ConsistentEntries()
    {
        var id = "tier-id";
        _dataSource.DevicesTiersIds.Add(id);
        _dataSource.QuotasTiersIds.Add(id);

        await _consistencyCheck.Run_for_DevicesTiers_vs_QuotasTiers(CancellationToken.None);

        _reporter.ReportedOrphanedTierIdOnDevices.Should().BeEmpty();
        _reporter.ReportedOrphanedTierIdOnQuotas.Should().BeEmpty();
    }

    #endregion
}
