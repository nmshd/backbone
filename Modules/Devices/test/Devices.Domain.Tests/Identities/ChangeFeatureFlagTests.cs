using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class ChangeFeatureFlagTests : AbstractTestsBase
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

    [Fact]
    public void Adds_Non_Existing_Feature_Flag()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var newName = FeatureFlagName.Parse("New");
        var featureFlags = new Dictionary<FeatureFlagName, bool>
        {
            { newName, true }
        };

        
        // Act
        identity.ChangeFeatureFlags(featureFlags);
        
        // Assert
        identity.FeatureFlags.Should().ContainKey(newName);
    }
}
