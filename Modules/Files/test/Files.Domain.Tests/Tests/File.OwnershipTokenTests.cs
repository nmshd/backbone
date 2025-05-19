using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests;

public class OwnershipTokenTests
{
    [Fact]
    public void A_files_ownership_should_initially_not_be_locked()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        file.OwnershipIsLocked.Should().BeFalse();
    }

    [Fact]
    public void A_file_is_always_created_with_an_ownership_token()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        file.OwnershipToken.Should().NotBeNull();
        file.OwnershipToken.Value.Length.Should().Be(20);
    }

    [Fact]
    public void File_should_be_able_to_generate_new_ownership_token()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var oldToken = file.OwnershipToken;

        file.RegenerateOwnershipToken();

        file.OwnershipToken.Should().NotBeEquivalentTo(oldToken);
        file.OwnershipIsLocked.Should().BeFalse();
    }
}
