using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Tooling;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Identities;

public class QuotaPeriodTests : AbstractTestsBase
{
    [Theory]
    [InlineData("2023-01-01T13:45:00.000", QuotaPeriod.Hour, "2023-01-01T13:00:00.000")]
    [InlineData("2023-01-01T13:45:00.000", QuotaPeriod.Day, "2023-01-01T00:00:00.000")]
    [InlineData("2023-06-01T12:00:00.000", QuotaPeriod.Week, "2023-05-29T00:00:00.000")]
    [InlineData("2020-01-03T12:00:00.000", QuotaPeriod.Week, "2019-12-30T00:00:00.000")]
    [InlineData("2020-02-03T12:00:00.000", QuotaPeriod.Month, "2020-02-01T00:00:00.000")]
    [InlineData("2020-01-03T12:00:00.000", QuotaPeriod.Year, "2020-01-01T00:00:00.000")]
    public void Begin(string currentDateString, QuotaPeriod quotaPeriod, string targetDate)
    {
        // Arrange
        var currentDate = DateTime.Parse(currentDateString);
        SystemTime.Set(currentDate);

        // Act
        var start = quotaPeriod.CalculateBegin(SystemTime.UtcNow);

        // Assert
        start.ShouldBe(DateTime.Parse(targetDate));
    }

    [Theory]
    [InlineData("2023-01-01T13:45:00.000", QuotaPeriod.Hour, "2023-01-01T13:59:59.999")]
    [InlineData("2023-01-01T13:45:00.000", QuotaPeriod.Day, "2023-01-01T23:59:59.999")]
    [InlineData("2020-01-01T12:00:00.000", QuotaPeriod.Week, "2020-01-05T23:59:59.999")]
    [InlineData("2024-01-03T12:00:00.000", QuotaPeriod.Week, "2024-01-07T23:59:59.999")]
    [InlineData("2024-02-03T12:00:00.000", QuotaPeriod.Month, "2024-02-29T23:59:59.999")]
    [InlineData("2024-02-03T12:00:00.000", QuotaPeriod.Year, "2024-12-31T23:59:59.999")]
    public void End(string currentDateString, QuotaPeriod quotaPeriod, string targetDate)
    {
        // Arrange
        var currentDate = DateTime.Parse(currentDateString);
        SystemTime.Set(currentDate);

        // Act
        var start = quotaPeriod.CalculateEnd(SystemTime.UtcNow);

        // Assert
        start.ShouldBe(DateTime.Parse(targetDate));
    }
}
