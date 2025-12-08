using Backbone.Modules.Files.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Domain.Tests.Helpers;
using Backbone.UnitTestTools.Shouldly.Extensions;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class ClaimOwnershipTests : AbstractTestsBase
{
    [Fact]
    public void File_ownership_can_be_claimed_using_the_correct_token()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        var initialToken = file.OwnershipToken;
        var claimingIdentity = CreateRandomIdentityAddress();

        // Act
        var result = file.ClaimOwnership(initialToken, claimingIdentity);

        // Assert
        result.ShouldBe(File.ClaimFileOwnershipResult.Ok);
        file.LastOwnershipClaimAt.ShouldNotBeNull();
        file.Owner.ShouldBe(claimingIdentity);
        file.OwnershipToken.ShouldNotBe(initialToken);
    }

    [Fact]
    public void Cannot_claim_ownership_for_own_file()
    {
        // Arrange
        var owner = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(owner);

        // Act
        var result = file.ClaimOwnership(file.OwnershipToken, owner);

        // Assert
        result.ShouldBe(File.ClaimFileOwnershipResult.CannotClaimOwnFile);
    }

    [Fact]
    public void Claiming_a_file_ownership_with_the_wrong_token_locks_it()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        var initialToken = file.OwnershipToken;
        var claimingIdentity = CreateRandomIdentityAddress();

        // Act
        var result = file.ClaimOwnership(FileOwnershipToken.New(), claimingIdentity);

        // Assert
        file.OwnershipIsLocked.ShouldBeTrue();
        result.ShouldBe(File.ClaimFileOwnershipResult.IncorrectToken);

        file.LastOwnershipClaimAt.ShouldBeNull();
        file.Owner.ShouldNotBe(claimingIdentity);
        file.OwnershipToken.ShouldBe(initialToken);
    }

    [Fact]
    public void A_file_with_locked_ownership_cannot_be_claimed()
    {
        // Arrange
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);
        var initialToken = file.OwnershipToken;
        file.ClaimOwnership(FileOwnershipToken.New(), CreateRandomIdentityAddress());

        // Act
        var result = file.ClaimOwnership(initialToken, CreateRandomIdentityAddress());

        // Assert
        result.ShouldBe(File.ClaimFileOwnershipResult.Locked);

        file.LastOwnershipClaimAt.ShouldBeNull();
        file.Owner.ShouldBe(identity);
        file.OwnershipToken.ShouldBe(initialToken);
    }

    [Fact]
    public void Raises_FileOwnershipClaimedDomainEvent()
    {
        // Arrange
        var originalOwner = CreateRandomIdentityAddress();
        var newOwner = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(originalOwner);

        // Act
        file.ClaimOwnership(file.OwnershipToken, newOwner);

        // Assert
        var domainEvent = file.ShouldHaveASingleDomainEvent<FileOwnershipClaimedDomainEvent>();
        domainEvent.FileId.ShouldBe(file.Id.Value);
        domainEvent.NewOwnerAddress.ShouldBe(newOwner.Value);
        domainEvent.OldOwnerAddress.ShouldBe(originalOwner.Value);
    }
}
