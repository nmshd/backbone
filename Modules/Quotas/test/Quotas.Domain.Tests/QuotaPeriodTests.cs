using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Tests.Extensions;
using Enmeshed.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class QuotaPeriodTests
{
    [Theory]
    [InlineData("2023-01-01T13:45:00.000", "2023-01-01T13:00:00.000", QuotaPeriod.Hour)]
    [InlineData("2023-01-01T13:45:00.000", "2023-01-01T00:00:00.000", QuotaPeriod.Day)]
    [InlineData("2023-06-01T12:00:00.000", "2023-05-29T00:00:00.000", QuotaPeriod.Week)]
    [InlineData("2020-01-03T12:00:00.000", "2019-12-30T00:00:00.000", QuotaPeriod.Week)]
    [InlineData("2020-02-03T12:00:00.000", "2020-02-01T00:00:00.000", QuotaPeriod.Month)]
    [InlineData("2020-01-03T12:00:00.000", "2020-01-01T00:00:00.000", QuotaPeriod.Year)]
    public void Begin(string currentDateString, string targetDate, QuotaPeriod quotaPeriod)
    {
        // Arrange
        var currentDate = DateTime.Parse(currentDateString);
        SystemTime.Set(currentDate);

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Should().Be(targetDate);
    }

    [Theory]
    [InlineData("2023-01-01T13:45:00.000", "2023-01-01T13:59:59.999", QuotaPeriod.Hour)]
    [InlineData("2023-01-01T13:45:00.000", "2023-01-01T23:59:59.999", QuotaPeriod.Day)]
    [InlineData("2020-01-01T12:00:00.000", "2020-01-05T23:59:59.999", QuotaPeriod.Week)]
    [InlineData("2024-01-03T12:00:00.000", "2024-01-07T23:59:59.999", QuotaPeriod.Week)]
    [InlineData("2024-02-03T12:00:00.000", "2024-02-29T23:59:59.999", QuotaPeriod.Month)]
    [InlineData("2024-02-03T12:00:00.000", "2024-12-31T23:59:59.999", QuotaPeriod.Year)]
    public void End(string currentDateString, string targetDate, QuotaPeriod quotaPeriod)
    {
        // Arrange
        var currentDate = DateTime.Parse(currentDateString);
        SystemTime.Set(currentDate);

        // Act
        var start = quotaPeriod.CalculateEnd();

        // Assert
        start.Should().Be(targetDate);
    }
}