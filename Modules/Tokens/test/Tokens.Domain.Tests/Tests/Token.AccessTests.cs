using Backbone.Modules.Tokens.Domain.DomainEvents;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests;

public class TokenAccessTests : AbstractTestsBase
{
    [Fact]
    public void An_identity_with_correct_access_gets_an_allocation()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();

        // Act
        var result = token.TryToAccess(identityAddress, deviceId, password);

        // Assert
        result.Should().Be(TokenAccessResult.AllocationAdded);
        token.Allocations.Should().HaveCount(1);
        token.Allocations[0].AllocatedBy.Should().Be(identityAddress);
        token.Allocations[0].AllocatedByDevice.Should().Be(deviceId);
    }

    [Fact]
    public void An_identity_can_not_access_a_token_without_a_password()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var wrongPassword = Convert.FromBase64String("wordpass");

        // Arrange
        var result = token.TryToAccess(identityAddress, deviceId, wrongPassword);

        // Act
        result.Should().Be(TokenAccessResult.WrongPassword);
    }

    [Fact]
    public void A_token_without_password_can_always_be_accessed()
    {
        // Arrange
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();

        // Act
        var result = token.TryToAccess(identityAddress, deviceId, null);

        // Assert
        result.Should().Be(TokenAccessResult.AllocationAdded);
    }

    [Fact]
    public void An_identity_can_not_be_allocated_twice()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        token.TryToAccess(identityAddress, deviceId, password);

        // Act
        var result = token.TryToAccess(identityAddress, deviceId, password);

        // Assert
        result.Should().Be(TokenAccessResult.Ok);
    }

    [Fact]
    public void An_allocated_identity_can_access_the_token_without_a_password()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        token.TryToAccess(identityAddress, deviceId, password);

        // Act
        var result = token.TryToAccess(identityAddress, deviceId, null);

        // Assert
        result.Should().Be(TokenAccessResult.Ok);
    }

    [Fact]
    public void The_owner_can_access_the_token_without_a_password()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var token = TestData.CreateToken(identityAddress, null, password);

        // Act
        var result = token.TryToAccess(identityAddress, deviceId, null);

        // Assert
        result.Should().Be(TokenAccessResult.Ok);
    }

    // Locked tokens

    [Fact]
    public void Enough_wrong_accesses_lock_the_token()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        SendInvalidAccessRequestsToToken(token, 99);

        // Act
        var result = token.TryToAccess(null, null, null);

        // Assert
        result.Should().Be(TokenAccessResult.Locked);
    }

    [Fact]
    public void Locking_a_token_raises_a_domain_event()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        token.ClearDomainEvents();
        SendInvalidAccessRequestsToToken(token, 99);

        // Act
        token.TryToAccess(null, null, null);

        // Assert
        token.IsLocked.Should().BeTrue();
        token.DomainEvents.Should().HaveCount(1);
        token.DomainEvents[0].Should().BeOfType<TokenLockedDomainEvent>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void An_identity_with_allocation_can_access_a_locked_token_with_and_without_a_password(bool usePassword)
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        token.TryToAccess(identityAddress, deviceId, password);
        LockToken(token);

        // Act
        var result = token.TryToAccess(identityAddress, deviceId, usePassword ? password : null);

        // Assert
        result.Should().Be(TokenAccessResult.Ok);
    }

    [Fact]
    public void An_identity_without_allocation_can_not_access_a_locked_token()
    {
        // Arrange
        var password = Convert.FromBase64String("password");
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, password);
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        LockToken(token);

        // Act
        var result = token.TryToAccess(identityAddress, deviceId, password);

        // Assert
        result.Should().Be(TokenAccessResult.Locked);
    }

    private static void LockToken(Token token) => SendInvalidAccessRequestsToToken(token, 100);

    private static void SendInvalidAccessRequestsToToken(Token token, int numberOfRequests)
    {
        for (var i = 0; i < numberOfRequests; i++)
            token.TryToAccess(null, null, null);
    }
}
