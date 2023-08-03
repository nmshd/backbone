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

    [Fact]
    public async void SanityCheckNoEntries()
    {
        await _consistencyCheck.Run_for_DevicesIdentities_vs_QuotasIdentities(CancellationToken.None);

        _reporter.ReportedOrphanedIdentityIdOnDevices.Should().BeEmpty();
        _reporter.ReportedOrphanedIdentityIdOnQuotas.Should().BeEmpty();
    }
}
