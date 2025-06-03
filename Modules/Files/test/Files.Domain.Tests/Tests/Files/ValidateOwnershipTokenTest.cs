using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Files.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Domain.Tests.Helpers;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class ValidateOwnershipTokenTest
{
    [Fact]
    public void Successful_validation_by_owner_returns_true()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();

        // Act
        var validationResult = file.ValidateFileOwnershipToken(file.OwnershipToken, file.Owner);

        // Assert
        validationResult.ShouldBeTrue();
    }

    [Fact]
    public void Unsuccessful_validation_by_owner_returns_false()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        var incorrectOwnershipToken = FileOwnershipToken.New();

        // Act
        var validationResult = file.ValidateFileOwnershipToken(incorrectOwnershipToken, file.Owner);

        // Assert
        validationResult.ShouldBeFalse();
    }

    [Fact]
    public void Successful_validation_by_non_owner_returns_true()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        var validatingIdentity = CreateRandomIdentityAddress();

        // Act
        var validationResult = file.ValidateFileOwnershipToken(file.OwnershipToken, validatingIdentity);

        // Assert
        validationResult.ShouldBeTrue();
    }

    [Fact]
    public void Unsuccessful_validation_by_non_owner_returns_false()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        var incorrectOwnershipToken = FileOwnershipToken.New();
        var validatingIdentity = CreateRandomIdentityAddress();

        // Act
        var validationResult = file.ValidateFileOwnershipToken(incorrectOwnershipToken, validatingIdentity);

        // Assert
        validationResult.ShouldBeFalse();
        file.OwnershipIsLocked.ShouldBeTrue();
    }

    [Fact]
    public void Unsuccessful_validation_by_non_owner_raises_FileOwnershipLockedDomainEvent()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        var incorrectOwnershipToken = FileOwnershipToken.New();
        var validatingIdentity = CreateRandomIdentityAddress();

        // Act
        file.ValidateFileOwnershipToken(incorrectOwnershipToken, validatingIdentity);

        // Assert
        file.ShouldHaveASingleDomainEvent<FileOwnershipLockedDomainEvent>();
    }

    [Fact]
    public void A_file_with_locked_ownership_returns_false_when_validating_ownership_token()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        file.ClaimOwnership(FileOwnershipToken.New(), file.Owner);

        // Act
        var validationResult = file.ValidateFileOwnershipToken(file.OwnershipToken, file.Owner);

        // Assert
        validationResult.ShouldBeFalse();
    }
}
