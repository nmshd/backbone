using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class IsGracePeriodOverTests : AbstractTestsBase
{
    [Fact]
    public void Returns_false_if_grace_period_is_not_over()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        // Act
        var result = identity.IsGracePeriodOver;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Returns_true_if_grace_period_is_over()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays + 1));

        // Act
        var result = identity.IsGracePeriodOver;

        // Assert
        result.Should().BeTrue();
    }
}
