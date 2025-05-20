using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Domain.Tests.Helpers;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class ClaimOwnershipTests
{
    [Fact]
    public void File_can_be_claimed_using_the_correct_token()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var initialToken = file.OwnershipToken;
        var claimingIdentity = CreateRandomIdentityAddress();
        var result = file.ClaimOwnership(initialToken, claimingIdentity);

        result.Should().Be(File.ClaimFileOwnershipResult.Ok);

        file.LastOwnershipClaimAt.Should().NotBeNull();
        file.Owner.Should().Be(claimingIdentity);
        file.OwnershipToken.Should().NotBe(initialToken);
    }

    [Fact]
    public void Claiming_a_file_with_the_wrong_token_locks_it()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var initialToken = file.OwnershipToken;
        var claimingIdentity = CreateRandomIdentityAddress();
        var result = file.ClaimOwnership(FileOwnershipToken.New(), claimingIdentity);

        result.Should().Be(File.ClaimFileOwnershipResult.IncorrectToken);

        file.LastOwnershipClaimAt.Should().BeNull();
        file.Owner.Should().NotBe(claimingIdentity);
        file.OwnershipToken.Should().Be(initialToken);
        file.OwnershipIsLocked.Should().BeTrue();
    }

    [Fact]
    public void A_file_with_locked_ownership_cannot_be_claimed()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var initialToken = file.OwnershipToken;
        var claimingIdentity = CreateRandomIdentityAddress();

        file.ClaimOwnership(FileOwnershipToken.New(), claimingIdentity);
        file.OwnershipIsLocked.Should().BeTrue();

        var result = file.ClaimOwnership(initialToken, claimingIdentity);
        result.Should().Be(File.ClaimFileOwnershipResult.Locked);

        file.LastOwnershipClaimAt.Should().BeNull();
        file.Owner.Should().Be(identity);
        file.OwnershipToken.Should().Be(initialToken);
    }
}
