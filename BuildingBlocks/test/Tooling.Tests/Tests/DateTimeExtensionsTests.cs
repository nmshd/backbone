using System.Collections;
using Enmeshed.Tooling.Extensions;
using Enmeshed.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Enmeshed.Tooling.Tests.Tests;
public class DateTimeExtensionsTests
{
    [Theory]
    [ClassData(typeof(DateTimeExtensionsTestData))]
    public void DateTimeExtensionMethods(string pivot, string expected, Func<DateTime, DateTime> operation)
    {
        // Arrange
        
        // Act
        var target = operation.Invoke(DateTime.Parse(pivot));

        // Assert
        target.Should().Be(expected);
    }

#pragma warning disable CS8974 // Converting method group to non-delegate type
    public class DateTimeExtensionsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "2023-02-23T00:00:30.000", "2023-02-23T00:00:00.000", DateTimeExtensions.StartOfDay };
            yield return new object[] { "2023-02-23T23:58:30.000", "2023-02-23T00:00:00.000", DateTimeExtensions.StartOfDay };
            yield return new object[] { "2023-06-20T00:00:30.000", "2023-06-19T00:00:00.000", DateTimeExtensions.StartOfWeek };
            yield return new object[] { "2023-01-01T23:58:30.000", "2022-12-26T00:00:00.000", DateTimeExtensions.StartOfWeek };
            yield return new object[] { "2023-01-01T00:00:30.000", "2023-01-01T00:00:00.000", DateTimeExtensions.StartOfMonth };
            yield return new object[] { "2023-02-23T00:00:30.000", "2023-02-01T00:00:00.000", DateTimeExtensions.StartOfMonth };
            yield return new object[] { "2023-02-23T00:00:30.000", "2023-01-01T00:00:00.000", DateTimeExtensions.StartOfYear };

            yield return new object[] { "2023-02-23T00:00:30.000", "2023-02-23T23:59:59.999", DateTimeExtensions.EndOfDay };
            yield return new object[] { "2023-02-23T23:58:30.000", "2023-02-23T23:59:59.999", DateTimeExtensions.EndOfDay };
            yield return new object[] { "2023-06-20T00:00:30.000", "2023-06-25T23:59:59.999", DateTimeExtensions.EndOfWeek };
            yield return new object[] { "2023-01-01T23:58:30.000", "2023-01-01T23:59:59.999", DateTimeExtensions.EndOfWeek };
            yield return new object[] { "2023-01-01T00:00:30.000", "2023-01-31T23:59:59.999", DateTimeExtensions.EndOfMonth };
            yield return new object[] { "2023-02-23T00:00:30.000", "2023-02-28T23:59:59.999", DateTimeExtensions.EndOfMonth };
            yield return new object[] { "2024-02-23T00:00:30.000", "2024-02-29T23:59:59.999", DateTimeExtensions.EndOfMonth };
            yield return new object[] { "2023-02-23T00:00:30.000", "2023-12-31T23:59:59.999", DateTimeExtensions.EndOfYear };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
#pragma warning restore CS8974 // Converting method group to non-delegate type
}
