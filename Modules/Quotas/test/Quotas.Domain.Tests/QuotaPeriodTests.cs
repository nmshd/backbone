using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
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
        start.Second.Should().Be(0);
        start.Minute.Should().Be(0);
        start.Hour.Should().Be(13);
        start.Day.Should().Be(01);
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
        start.Second.Should().Be(0);
        start.Minute.Should().Be(0);
        start.Hour.Should().Be(0);
        start.Day.Should().Be(01);
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
        start.Second.Should().Be(0);
        start.Minute.Should().Be(0);
        start.Hour.Should().Be(0);
        start.Day.Should().Be(28);
        start.Month.Should().Be(5);
        start.Year.Should().Be(2023);
    }

[Fact]
    public void Begin_Week_quota_at_jan_3rd_2020()
    {
        // Arrange
        var currentDate = new DateTime(2020, 01, 01, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Week;

        // Act
        var start = quotaPeriod.CalculateBegin();

        // Assert
        start.Second.Should().Be(0);
        start.Minute.Should().Be(0);
        start.Hour.Should().Be(0);
        start.Day.Should().Be(29);
        start.Month.Should().Be(12);
        start.Year.Should().Be(2019);
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
        end.Millisecond.Should().Be(999);
        end.Second.Should().Be(59);
        end.Minute.Should().Be(59);
        end.Hour.Should().Be(13);
        end.Day.Should().Be(01);
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
        end.Second.Should().Be(59);
        end.Minute.Should().Be(59);
        end.Hour.Should().Be(23);
        end.Day.Should().Be(01);
    }

    [Fact]
    public void End_Week_quota_at_jan_3rd_2020()
    {
        // Arrange
        var currentDate = new DateTime(2020, 01, 01, 12, 00, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);
        var quotaPeriod = QuotaPeriod.Week;

        // Act
        var start = quotaPeriod.CalculateEnd();

        // Assert
        start.Second.Should().Be(59);
        start.Minute.Should().Be(59);
        start.Hour.Should().Be(23);
        start.Day.Should().Be(4);
        start.Month.Should().Be(1);
        start.Year.Should().Be(2020);
    }

}
