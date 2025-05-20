using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class RegenerateOwnershipToken
{
    [Fact]
    public void File_should_be_able_to_generate_new_ownership_token()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var oldToken = file.OwnershipToken;

        file.RegenerateOwnershipToken(file.Owner);

        file.OwnershipToken.Should().NotBeEquivalentTo(oldToken);
        file.OwnershipIsLocked.Should().BeFalse();
    }

    [Fact]
    public void A_ownership_token_can_only_be_regenerated_by_its_owner()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var acting = () => file.RegenerateOwnershipToken(CreateRandomIdentityAddress());
        acting.Should().Throw<DomainActionForbiddenException>();
    }

    [Fact]
    public void A_successful_ownership_token_regeneration_unlocks_the_file_ownership()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        file.ClaimOwnership(FileOwnershipToken.New(), file.Owner);
        file.OwnershipIsLocked.Should().BeTrue();

        file.RegenerateOwnershipToken(file.Owner);
        file.OwnershipIsLocked.Should().BeFalse();
    }
}
