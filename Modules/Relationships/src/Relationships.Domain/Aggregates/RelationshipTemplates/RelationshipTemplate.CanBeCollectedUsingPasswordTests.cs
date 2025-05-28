using Backbone.Modules.Relationships.Domain.TestHelpers;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateCanBeCollectedUsingPasswordExpressionTests : AbstractTestsBase
{
    [Fact]
    public void Can_collect_without_a_password_when_no_password_is_defined()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, null);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Can_collect_with_correct_password()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, [1]);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Cannot_collect_with_incorrect_password()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, [2]);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Can_collect_as_owner_without_a_password()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(creator, null);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Can_collect_without_password_when_template_is_already_allocated_by_me()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);
        template.AllocateFor(collector, CreateRandomDeviceId());

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, null);

        // Assert
        result.ShouldBeTrue();
    }
}
