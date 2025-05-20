using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class ValidateOwnershipTokenTest
{
    [Fact]
    public void Token_can_be_validated_by_its_owner()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var resultCorrectToken = file.ValidateFileOwnershipTokenForCorrectness(file.OwnershipToken, file.Owner);
        resultCorrectToken.Should().BeTrue();

        var wrongCorrectToken = file.ValidateFileOwnershipTokenForCorrectness(FileOwnershipToken.New(), file.Owner);
        wrongCorrectToken.Should().BeTrue();
    }

    [Fact]
    public void A_file_with_locked_ownership_returns_false_when_validating_ownership_token()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        file.ClaimOwnership(FileOwnershipToken.New(), file.Owner);
        file.OwnershipIsLocked.Should().BeTrue();

        var result = file.ValidateFileOwnershipTokenForCorrectness(file.OwnershipToken, file.Owner);
        result.Should().BeFalse();
    }

    [Fact]
    public void A_ownership_token_can_only_be_validated_by_its_owner()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var acting = () => file.ValidateFileOwnershipTokenForCorrectness(file.OwnershipToken, CreateRandomIdentityAddress());
        acting.Should().Throw<DomainActionForbiddenException>();
    }
}
