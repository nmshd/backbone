﻿using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

namespace Backbone.BuildingBlocks.Application.Tests.Extensions;

public class ExtensionsTests : AbstractTestsBase
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
        result.Should().BeTrue();
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
        result.Should().BeFalse();
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
        result.Should().BeFalse();
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
        result.Should().BeTrue();
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
        result.Should().BeTrue();
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
        result.Should().BeTrue();
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
        result.Should().BeFalse();
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
