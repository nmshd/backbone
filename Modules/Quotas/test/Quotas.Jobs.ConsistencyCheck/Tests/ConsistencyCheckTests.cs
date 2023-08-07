using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.DataSource;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Infrastructure.Reporter;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Tests.Tests;

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

        _reporter.ReportedIdentitiesMissingFromQuotas.Should().BeEmpty();
        _reporter.ReportedIdentitiesMissingFromDevices.Should().BeEmpty();
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_ConsistentEntries()
    {
        const string id = "identity-id";
        _dataSource.DevicesIdentitiesIds.Add(id);
        _dataSource.QuotasIdentitiesIds.Add(id);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedIdentitiesMissingFromQuotas.Should().BeEmpty();
        _reporter.ReportedIdentitiesMissingFromDevices.Should().BeEmpty();
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_MissingFromDevices()
    {
        const string id = "identity-id";

        _dataSource.QuotasIdentitiesIds.Add(id);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedIdentitiesMissingFromQuotas.Should().BeEmpty();
        _reporter.ReportedIdentitiesMissingFromDevices.Should().HaveCount(1);
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_MissingFromQuotas()
    {
        const string id = "identity-id";

        _dataSource.DevicesIdentitiesIds.Add(id);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedIdentitiesMissingFromDevices.Should().BeEmpty();
        _reporter.ReportedIdentitiesMissingFromQuotas.Should().HaveCount(1);
    }

    [Fact]
    public async void SanityCheck_DevicesIdentities_vs_QuotasIdentities_MissingFromBothSides()
    {
        const string idCommon = "identity-id-common";
        const string idOnlyInDevices = "identity-id-devices";
        const string idOnlyInQuotas = "identity-id-quotas";

        _dataSource.DevicesIdentitiesIds.Add(idCommon);
        _dataSource.QuotasIdentitiesIds.Add(idCommon);

        _dataSource.DevicesIdentitiesIds.Add(idOnlyInDevices);

        _dataSource.QuotasIdentitiesIds.Add(idOnlyInQuotas);

        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedIdentitiesMissingFromQuotas.Single().Should().Be(idOnlyInDevices);
        _reporter.ReportedIdentitiesMissingFromDevices.Single().Should().Be(idOnlyInQuotas);
    }

    #endregion

    #region DevicesTiers_vs_QuotasTiers

    [Fact]
    public async void SanityCheck_DevicesTiers_vs_QuotasTiers_NoEntries()
    {
        await _consistencyCheck.Run_for_DevicesTiers_vs_QuotasTiers(CancellationToken.None);

        _reporter.ReportedTiersMissingFromQuotas.Should().BeEmpty();
        _reporter.ReportedTiersMissingFromDevices.Should().BeEmpty();
    }

    [Fact]
    public async void SanityCheck_DevicesTiers_vs_QuotasTiers_ConsistentEntries()
    {
        const string id = "tier-id";
        _dataSource.DevicesTiersIds.Add(id);
        _dataSource.QuotasTiersIds.Add(id);

        await _consistencyCheck.Run_for_DevicesTiers_vs_QuotasTiers(CancellationToken.None);

        _reporter.ReportedTiersMissingFromQuotas.Should().BeEmpty();
        _reporter.ReportedTiersMissingFromDevices.Should().BeEmpty();
    }

    #endregion

    #region Run_for_TierQuotaDefinitions_vs_TierQuotas

    [Fact]
    public async void SanityCheck_TierQuotaDefinitions_vs_TierQuotas_NoEntries()
    {
        await _consistencyCheck.Run_for_DevicesTiers_vs_QuotasTiers(CancellationToken.None);

        _reporter.ReportedTierQuotasMissingFromTier.Should().BeEmpty();
        _reporter.ReportedTierQuotaDefinitionMissingFromIdentities.Should().BeEmpty();
    }

    #endregion
}
