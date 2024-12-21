using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;
using Backbone.UnitTestTools.Extensions;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests;

public class TokenAllocationTests : AbstractTestsBase
{
    [Fact]
    public void An_identity_can_be_allocated()
    {
        // Arrange
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();

        // Act
        token.AddAllocationFor(identityAddress, deviceId);

        // Assert
        token.Allocations.Should().HaveCount(1);
        token.Allocations[0].AllocatedBy.Should().Be(identityAddress);
        token.Allocations[0].AllocatedByDevice.Should().Be(deviceId);
    }

    [Fact]
    public void An_identity_can_not_be_allocated_twice()
    {
        // Arrange
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        token.AddAllocationFor(identityAddress, deviceId);

        // Act
        var acting = () => token.AddAllocationFor(identityAddress, deviceId);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.token.alreadyAllocated");
    }

    [Fact]
    public void The_owner_can_not_be_allocated()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var token = TestData.CreateToken(identityAddress, null);

        // Act
        var acting = () => token.AddAllocationFor(identityAddress, deviceId);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.token.noAllocationForOwner");
    }
}
