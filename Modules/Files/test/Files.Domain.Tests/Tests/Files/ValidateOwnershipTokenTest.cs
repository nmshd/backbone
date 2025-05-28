using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class ValidateOwnershipTokenTest
{
    [Fact]
    public void Token_can_be_validated_by_its_owner()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();

        // Act
        var resultCorrectToken = file.ValidateFileOwnershipToken(file.OwnershipToken, file.Owner);

        // Assert
        resultCorrectToken.ShouldBeTrue();
    }

    [Fact]
    public void A_file_with_locked_ownership_returns_false_when_validating_ownership_token()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        file.ClaimOwnership(FileOwnershipToken.New(), file.Owner);

        // Act
        var result = file.ValidateFileOwnershipToken(file.OwnershipToken, file.Owner);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void A_ownership_token_can_only_be_validated_by_its_owner()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();

        // Act
        Func<object> acting = () => file.ValidateFileOwnershipToken(file.OwnershipToken, CreateRandomIdentityAddress());

        // Assert
        acting.ShouldThrow<DomainActionForbiddenException>();
    }
}
