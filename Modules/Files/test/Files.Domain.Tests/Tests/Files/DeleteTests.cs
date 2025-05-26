using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests.Files;

public class DeleteTests : AbstractTestsBase
{
    [Fact]
    public void File_can_be_deleted_by_its_owner()
    {
        // Arrange
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        // Act
        var acting = () => file.EnsureCanBeDeletedBy(identity);

        // Assert
        acting.Should().NotThrow();
    }

    [Fact]
    public void File_can_not_be_deleted_by_others()
    {
        // Arrange
        var nonCreatingIdentity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile();

        // Act
        var acting = () => file.EnsureCanBeDeletedBy(nonCreatingIdentity);

        // Assert
        acting.Should().Throw<DomainActionForbiddenException>();
    }
}
