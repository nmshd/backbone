using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;

public class ApplicationUserTests : AbstractTestsBase
{
    [Fact]
    public void Login_occurred()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        var user = new ApplicationUser(device);

        // Act
        user.LoginOccurred();

        // Assert
        user.HasLoggedIn.Should().BeTrue();
    }
}
