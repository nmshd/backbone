namespace Backbone.Modules.Devices.Application.Tests;

public class SetTest
{
    [Fact]
    public void Egal()
    {
        // Arrange
        List<string> list1 = ["a", "b"];
        List<string> list2 = ["c", "b"];

        // Act
        var result = list1.Concat(list2).ToHashSet();

        // Assert
        result.Should().Contain("a");
        result.Should().Contain("b");
        result.Should().Contain("c");
        result.Should().HaveCount(3);
    }
}
