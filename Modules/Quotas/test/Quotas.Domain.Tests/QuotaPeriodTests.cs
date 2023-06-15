using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Tests.Extensions;
using Enmeshed.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class QuotaPeriodTests
{
    [Fact]
    public void Begin_hour_quota_at_13_45()
    {
        // Arrange
        var currentDate = new DateTime(2023, 01, 01, 13, 45, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Hour;

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Should().Be("2023-01-01T13:00:00.000");
    }

    [Fact]
    public void Begin_day_quota_at_jan_1st()
    {
        // Arrange
        var currentDate = new DateTime(2023, 01, 01, 13, 45, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Day;

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Should().Be("2023-01-01T00:00:00.000");
    }

    [Fact]
    public void Begin_Week_quota_at_jun_1st_2023()
    {
        // Arrange
        var currentDate = new DateTime(2023, 06, 01, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Week;

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Should().Be("2023-05-29T00:00:00.000");
    }

    [Fact]
    public void Begin_Week_quota_at_jan_3rd_2020()
    {
        // Arrange
        var currentDate = new DateTime(2020, 01, 03, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Week;

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Should().Be("2019-12-30T00:00:00.000");
    }

    [Fact]
    public void Begin_Month_quota_at_feb_3rd_2020()
    {
        // Arrange
        var currentDate = new DateTime(2020, 02, 03, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Month;

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Should().Be("2020-02-01T00:00:00.000");
    }

    [Fact]
    public void Begin_Year_quota_at_jan_3rd_2020()
    {
        // Arrange
        var currentDate = new DateTime(2020, 01, 03, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Year;

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Should().Be("2020-01-01T00:00:00.000");
    }

    [Fact]
    public void End_hour_quota_at_13_45()
    {
        // Arrange
        var currentDate = new DateTime(2023, 01, 01, 13, 45, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Hour;

        // Act
        var end = quotaPeriod.CalculateEnd();

        // Assert
        end.Should().Be("2023-01-01T13:59:59.999");
    }

    [Fact]
    public void End_day_quota_at_jan_1st()
    {
        // Arrange
        var currentDate = new DateTime(2023, 01, 01, 13, 45, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Day;

        // Act
        var end = quotaPeriod.CalculateEnd();

        // Assert
        end.Should().Be("2023-01-01T23:59:59.999");
    }

    [Fact]
    public void End_Week_quota_at_jan_3rd_2020()
    {
        // Arrange
        var currentDate = new DateTime(2020, 01, 01, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Week;

        // Act
        var end = quotaPeriod.CalculateEnd();

        // Assert
        end.Should().Be("2020-01-05T23:59:59.999");
    }

    [Fact]
    public void End_Week_quota_at_jan_3rd_2024()
    {
        // Arrange
        var currentDate = new DateTime(2024, 01, 03, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Week;

        // Act
        var end = quotaPeriod.CalculateEnd();

        // Assert
        end.Should().Be("2024-01-07T23:59:59.999");
    }

    [Fact]
    public void End_Month_quota_at_feb_3rd_2024()
    {
        // Arrange
        var currentDate = new DateTime(2024, 02, 03, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Month;

        // Act
        var end = quotaPeriod.CalculateEnd();

        // Assert
        end.Should().Be("2024-02-29T23:59:59.999");
    }
}