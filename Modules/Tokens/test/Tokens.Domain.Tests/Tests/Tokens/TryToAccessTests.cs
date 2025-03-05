using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;
using Backbone.Tooling;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests.Tokens;

public partial class TokenTryToAccessAccessTests : AbstractTestsBase
{
    private static readonly IdentityAddress IDENTITY_A = CreateRandomIdentityAddress();
    private static readonly IdentityAddress IDENTITY_B = CreateRandomIdentityAddress();
    private static readonly IdentityAddress IDENTITY_C = CreateRandomIdentityAddress();

    private static readonly byte[] PASSWORD_X = [0x01];
    private static readonly byte[] PASSWORD_Y = [0x02];

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

    [Theory]
    [ClassData(typeof(TokenAccessTestData))]
    public void All_possible_scenarios(int _, TokenProperties properties)
    {
        // Arrange
        var createdBy = TranslateIdentity(properties.CreatedBy);
        var forIdentity = TranslateIdentity(properties.ForIdentity);
        var activeIdentity = TranslateIdentity(properties.ActiveIdentity)!;

        var definedPassword = TranslatePassword(properties.DefinedPassword);
        var passwordOnGet = TranslatePassword(properties.PasswordOnGet);

        var token = TestData.CreateToken(createdBy!, forIdentity, definedPassword);

        if (properties.HasAllocation)
            token.TryToAccess(activeIdentity, CreateRandomDeviceId(), definedPassword);

        if (properties.IsLocked && definedPassword != null)
            LockToken(token, activeIdentity);

        var numberOfAllocationsBeforeAct = token.Allocations.Count;

        if (properties.IsExpired)
            SystemTime.Set(token.ExpiresAt.AddDays(1));

        // Act
        var result = token.TryToAccess(activeIdentity, CreateRandomDeviceId(), passwordOnGet);

        // Assert
        result.Should().Be(properties.ExpectedResult);

        var numberOfAllocationsAfterAct = token.Allocations.Count;

        var numberOfAddedAllocations = numberOfAllocationsAfterAct - numberOfAllocationsBeforeAct;

        if (result == TokenAccessResult.AllocationAdded)
            numberOfAddedAllocations.Should().Be(1);
        else
            numberOfAddedAllocations.Should().Be(0);
    }

    private static IdentityAddress? TranslateIdentity(Identity identityName)
    {
        return identityName switch
        {
            Identity.Anonymous => null,
            Identity.A => IDENTITY_A,
            Identity.B => IDENTITY_B,
            Identity.C => IDENTITY_C,
            _ => throw new ArgumentOutOfRangeException(nameof(identityName), identityName, null)
        };
    }

    private static byte[]? TranslatePassword(Password passwordName)
    {
        return passwordName switch
        {
            Password.Empty => null,
            Password.X => PASSWORD_X,
            Password.Y => PASSWORD_Y,
            _ => throw new ArgumentOutOfRangeException(nameof(passwordName), passwordName, null)
        };
    }


    private static void LockToken(Token token, IdentityAddress? addressUsedToLock = null)
    {
        SendInvalidAccessRequestsToToken(token, 100, addressUsedToLock);
    }

    private static void SendInvalidAccessRequestsToToken(Token token, int numberOfRequests, IdentityAddress? addressUsedToLock = null)
    {
        addressUsedToLock ??= CreateRandomIdentityAddress();

        for (var i = 0; i < numberOfRequests; i++)
            token.TryToAccess(addressUsedToLock, null, [0x09]);
    }
}
