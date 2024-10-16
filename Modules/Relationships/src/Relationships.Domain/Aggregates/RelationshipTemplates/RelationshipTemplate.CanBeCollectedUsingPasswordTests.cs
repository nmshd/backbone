using Backbone.Modules.Relationships.Domain.TestHelpers;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateCanBeCollectedUsingPasswordExpressionTests : AbstractTestsBase
{
    [Fact]
    public void Can_collect_without_a_password_when_no_password_is_defined()
    {
        // Arrange
        var creator = TestDataGenerator.CreateRandomIdentityAddress();
        var collector = TestDataGenerator.CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Can_collect_with_correct_password()
    {
        // Arrange
        var creator = TestDataGenerator.CreateRandomIdentityAddress();
        var collector = TestDataGenerator.CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, [1]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Cannot_collect_with_incorrect_password()
    {
        // Arrange
        var creator = TestDataGenerator.CreateRandomIdentityAddress();
        var collector = TestDataGenerator.CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, [2]);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Can_collect_as_owner_without_a_password()
    {
        // Arrange
        var creator = TestDataGenerator.CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(creator, null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Can_collect_without_password_when_template_is_already_allocated_by_me()
    {
        // Arrange
        var creator = TestDataGenerator.CreateRandomIdentityAddress();
        var collector = TestDataGenerator.CreateRandomIdentityAddress();

        var template = TestData.CreateRelationshipTemplate(creator, password: [1]);
        template.AllocateFor(collector, TestDataGenerator.CreateRandomDeviceId());

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, null);

        // Assert
        result.Should().BeTrue();
    }
}
