using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class IdentityTests
{
    [Fact]
    public void Can_create_identity_with_valid_properties()
    {
        // Arrange
        var address = "someAddress";
        var tierId = new TierId("TIRsomeTierId1111111");

        // Act
        var identity = new Identity(address, tierId);

        // Assert
        identity.Address.Should().Be(address);
        identity.TierId.Should().Be(tierId);
    }

    [Fact]
    public void Can_assign_tier_quota_from_definition_to_identity()
    {
        // Arrange
        var address = "someAddress";
        var tierId = new TierId("TIRsomeTierId1111111");
        var identity = new Identity(address, tierId);
        var max = 5;
        var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, max, QuotaPeriod.Month);

        // Act
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // no exception thrown signifies the test worked
    }
}
