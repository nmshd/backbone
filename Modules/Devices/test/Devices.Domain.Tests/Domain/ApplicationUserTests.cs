using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;

public class ApplicationUserTests : AbstractTestsBase
{
    [Fact]
    public void Login_occurred()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        var user = new ApplicationUser(identity, CommunicationLanguage.DEFAULT_LANGUAGE, device.Id);

        // Act
        user.LoginOccurred();

        // Assert
        user.HasLoggedIn.Should().BeTrue();
    }
}
