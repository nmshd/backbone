using System.Linq.Expressions;
using Backbone.Tooling.Extensions;

namespace Backbone.Tooling.Tests.Tests.Extensions;

public class ExpressionExtensionsTests : AbstractTestsBase
{
    [Fact]
    public void And_with_both_sides_true()
    {
        // Arrange
        var intWrapper = new IntWrapper(1);
        var compiledExpression = IntWrapper.IsValueLowerThan(2).And(IntWrapper.IsValueGreaterThan(0)).Compile();

        // Act
        var result = compiledExpression(intWrapper);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void And_with_only_right_side_true()
    {
        // Arrange
        var intWrapper = new IntWrapper(1);
        var compiledExpression = IntWrapper.IsValueLowerThan(1).And(IntWrapper.IsValueGreaterThan(0)).Compile();

        // Act
        var result = compiledExpression(intWrapper);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void And_with_only_left_side_true()
    {
        // Arrange
        var intWrapper = new IntWrapper(1);
        var compiledExpression = IntWrapper.IsValueLowerThan(2).And(IntWrapper.IsValueGreaterThan(1)).Compile();

        // Act
        var result = compiledExpression(intWrapper);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Or_with_only_right_side_true()
    {
        // Arrange
        var intWrapper = new IntWrapper(1);
        var compiledExpression = IntWrapper.IsValueLowerThan(1).Or(IntWrapper.IsValueGreaterThan(0)).Compile();

        // Act
        var result = compiledExpression(intWrapper);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Or_with_only_left_side_true()
    {
        // Arrange
        var intWrapper = new IntWrapper(1);
        var compiledExpression = IntWrapper.IsValueLowerThan(2).Or(IntWrapper.IsValueGreaterThan(1)).Compile();

        // Act
        var result = compiledExpression(intWrapper);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Or_with_both_sides_true()
    {
        // Arrange
        var intWrapper = new IntWrapper(1);
        var compiledExpression = IntWrapper.IsValueLowerThan(2).Or(IntWrapper.IsValueGreaterThan(0)).Compile();

        // Act
        var result = compiledExpression(intWrapper);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Or_with_both_sides_false()
    {
        // Arrange
        var intWrapper = new IntWrapper(1);
        var compiledExpression = IntWrapper.IsValueLowerThan(1).Or(IntWrapper.IsValueGreaterThan(1)).Compile();

        // Act
        var result = compiledExpression(intWrapper);

        // Assert
        result.ShouldBeFalse();
    }
}

public class IntWrapper
{
    public IntWrapper(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Expression<Func<IntWrapper, bool>> IsValueGreaterThan(int x)
    {
        return actual => actual.Value > x;
    }

    public static Expression<Func<IntWrapper, bool>> IsValueLowerThan(int x)
    {
        return actual => actual.Value < x;
    }
}
