using Backbone.Modules.Devices.Application.Extensions;
using FluentAssertions;
using Xunit;
using static System.Double;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Extensions;
public class IEnumerableExtensionsTests
{
    [Fact]
    public void Null_Parameter_Throws_ArgumentNullException()
    {
        // Act
        var acting = () => ((List<int>)null).Split(10); ;

        // Assert
        acting.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Empty_List_Returns_Empty_List_Of_Lists()
    {
        // Arrange
        var list = new List<int>().AsReadOnly();

        // Act
        var result = list.Split(10);

        // Assert
        result.Should().BeEmpty("The argument list was empty.");
    }

    [Fact]
    public void Large_List_Returns_Lists_Of_Lists()
    {
        // Arrange
        var list = new List<int>();
        for (var i = 0; i < 192; i++)
        {
            list.Add(i);
        }

        const int BATCH_SIZE = 10;

        // Act
        var result = list.Split(BATCH_SIZE);

        // Assert
        list.Count.Should().BeGreaterThan(BATCH_SIZE);
        result.Should().HaveCount((int)Ceiling((double)list.Count / BATCH_SIZE));
        result.Aggregate(0, (acc, x) => acc + x.Count()).Should().Be(list.Count);
    }

    [Fact]
    public void Small_List_Returns_Single_List_Of_Lists()
    {
        // Arrange
        var list = new List<int>();
        for (var i = 0; i < 5; i++)
        {
            list.Add(i);
        }

        const int BATCH_SIZE = 10;

        // Act
        var result = list.Split(BATCH_SIZE);

        // Assert
        list.Count.Should().BeLessThan(BATCH_SIZE);
        result.Should().HaveCount(1);
        result.Single().Count().Should().Be(list.Count);
    }

}
