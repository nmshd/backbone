using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;

public class ApplicationUserTests
{
    [Fact]
    public void Login_occurred()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        var user = new ApplicationUser(identity, device.Id);

        // Act
        user.LoginOccurred();

        // Assert
        user.HasLoggedIn.Should().BeTrue();
    }
}
