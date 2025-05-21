using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class RegenerateOwnershipToken
{
    [Fact]
    public void Generates_new_ownership_token()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        var oldToken = file.OwnershipToken;

        // Act
        file.RegenerateOwnershipToken(file.Owner);

        // Assert
        file.OwnershipToken.Should().NotBeEquivalentTo(oldToken);
    }

    [Fact]
    public void An_ownership_token_can_only_be_regenerated_by_its_owner()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();

        // Act
        var acting = () => file.RegenerateOwnershipToken(CreateRandomIdentityAddress());

        // Assert
        acting.Should().Throw<DomainActionForbiddenException>();
    }

    [Fact]
    public void A_successful_ownership_token_regeneration_unlocks_the_file_ownership()
    {
        // Arrange
        var file = TestDataGenerator.CreateFile();
        file.ClaimOwnership(FileOwnershipToken.New(), file.Owner);

        // Act
        file.RegenerateOwnershipToken(file.Owner);

        // Assert
        file.OwnershipIsLocked.Should().BeFalse();
    }
}
