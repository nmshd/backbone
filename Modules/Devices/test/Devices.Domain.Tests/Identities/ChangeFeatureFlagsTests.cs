using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.FluentAssertions.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class ChangeFeatureFlagsTests : AbstractTestsBase
{
    [Fact]
    public void Raises_FeatureFlagsOfIdentityChangedEvent()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var firstName = FeatureFlagName.Parse("First");
        var secondName = FeatureFlagName.Parse("Second");
        var featureFlags = new Dictionary<FeatureFlagName, bool>
        {
            { firstName, true },
            { secondName, false }
        };

        // Act
        identity.ChangeFeatureFlags(featureFlags);

        // Assert
        var domainEvent = identity.Should().HaveASingleDomainEvent<FeatureFlagsOfIdentityChangedDomainEvent>();
        domainEvent.IdentityAddress.Should().Be(identity.Address);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Adds_Non_Existing_Feature_Flag(bool isEnabled)
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var newName = FeatureFlagName.Parse("New");
        var featureFlags = new Dictionary<FeatureFlagName, bool>
        {
            { newName, isEnabled }
        };

        // Act
        identity.ChangeFeatureFlags(featureFlags);

        // Assert
        identity.FeatureFlags.Should().Contain(newName);
        identity.FeatureFlags.GetFeatureFlag(newName).IsEnabled.Should().Be(isEnabled);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void Changes_Existing_Feature_Flag(bool oldValue, bool newValue)
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var name = FeatureFlagName.Parse("First");
        var featureFlags = new Dictionary<FeatureFlagName, bool>
        {
            { name, oldValue }
        };
        identity.ChangeFeatureFlags(featureFlags);
        var featureFlagOverride = new Dictionary<FeatureFlagName, bool>
        {
            { name, newValue }
        };

        // Act
        identity.ChangeFeatureFlags(featureFlagOverride);

        // Assert
        identity.FeatureFlags.GetFeatureFlag(name).IsEnabled.Should().Be(newValue);
    }
}
