using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class CreationTests : AbstractTestsBase
{
    [Fact]
    public void A_files_ownership_is_not_locked_after_creation()
    {
        // Act
        var file = TestDataGenerator.CreateFile();

        // Assert
        file.OwnershipIsLocked.ShouldBeFalse();
    }

    [Fact]
    public void A_file_is_always_created_with_an_ownership_token()
    {
        // Act
        var file = TestDataGenerator.CreateFile();

        // Assert
        file.OwnershipToken.ShouldNotBeNull();
        file.OwnershipToken.Value.Length.ShouldBe(20);
    }
}
