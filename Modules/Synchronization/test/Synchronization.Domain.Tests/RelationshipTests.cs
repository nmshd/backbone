using Backbone.Modules.Synchronization.Domain.Entities.Relationships;

namespace Backbone.Modules.Synchronization.Domain.Tests;

public class RelationshipTests : AbstractTestsBase
{
    [Fact]
    public void IsBetween_returns_true_when_both_are_part_of_the_relationship()
    {
        // Arrange
        var relationship = new Relationship(RelationshipId.New(), CreateRandomIdentityAddress(), CreateRandomIdentityAddress(), RelationshipStatus.Active);

        // Act
        var result1 = relationship.IsBetween(relationship.From, relationship.To);
        var result2 = relationship.IsBetween(relationship.To, relationship.From);

        // Assert
        result1.ShouldBeTrue();
        result2.ShouldBeTrue();
    }

    [Fact]
    public void IsBetween_returns_false_when_at_least_one_is_not_part_of_the_relationship()
    {
        // Arrange
        var from = CreateRandomIdentityAddress();
        var to = CreateRandomIdentityAddress();
        var relationship = new Relationship(RelationshipId.New(), from, to, RelationshipStatus.Active);

        // Act
        var result1 = relationship.IsBetween(CreateRandomIdentityAddress(), to);
        var result2 = relationship.IsBetween(from, CreateRandomIdentityAddress());
        var result3 = relationship.IsBetween(CreateRandomIdentityAddress(), CreateRandomIdentityAddress());

        // Assert
        result1.ShouldBeFalse();
        result2.ShouldBeFalse();
        result3.ShouldBeFalse();
    }
}
