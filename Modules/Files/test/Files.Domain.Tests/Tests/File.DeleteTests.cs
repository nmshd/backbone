using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Files.Domain.Tests.Helpers;

namespace Backbone.Modules.Files.Domain.Tests.Tests;

public class FileDeleteTests : AbstractTestsBase
{
    [Fact]
    public void File_can_be_deleted_by_its_owner()
    {
        var identity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(identity);

        var acting = () => file.EnsureCanBeDeletedBy(identity);

        acting.Should().NotThrow();
    }

    [Fact]
    public void File_can_not_be_deleted_by_others()
    {
        var creatorIdentity = CreateRandomIdentityAddress();
        var otherIdentity = CreateRandomIdentityAddress();
        var file = TestDataGenerator.CreateFile(creatorIdentity);

        var acting = () => file.EnsureCanBeDeletedBy(otherIdentity);

        acting.Should().Throw<DomainActionForbiddenException>();
    }
}
