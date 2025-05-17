using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests;

public class File_OwnershipTokenTests
{
    [Fact]
    public void A_file_is_allways_created_with_an_ownership_token()
    {
        var identity = CreateRandomIdentityAddress();
        var file = FileCreationHelper.CreateFile(identity);

        file.OwnershipToken.Should().NotBeNull();
        file.OwnershipToken.Value.Length.Should().Be(20);
    }

    [Fact]
    public void File_should_be_able_to_generate_new_ownership_token()
    {
        var identity = CreateRandomIdentityAddress();
        var file = FileCreationHelper.CreateFile(identity);

        var oldToken = file.OwnershipToken;

        file.RegenerateOwnershipToken();

        file.OwnershipToken.Should().NotBeEquivalentTo(oldToken);
    }
}
