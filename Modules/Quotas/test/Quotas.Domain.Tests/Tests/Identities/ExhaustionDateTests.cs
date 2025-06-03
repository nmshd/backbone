using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Identities;

public class ExhaustionDateTests : AbstractTestsBase
{
    [Theory]
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0000", 0)] // equal
    [InlineData("2000-01-01T00:00:00.0000", "2021-01-01T00:00:00.0000", -1)] // compared date has higher year
    [InlineData("2000-01-01T00:00:00.0000", "1999-01-01T00:00:00.0000", 1)] // compared date has lower year
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0001", -1)] // compared date has higher millisecond
    [InlineData("2000-01-01T00:00:00.0001", "2000-01-01T00:00:00.0000", 1)] // compared date has lower millisecond
    public void CompareTo(string mainDate, string comparedDate, int expectedResult)
    {
        var parsedMainDate = new ExhaustionDate(DateTime.Parse(mainDate));
        var parsedComparedDate = new ExhaustionDate(DateTime.Parse(comparedDate));

        parsedMainDate.CompareTo(parsedComparedDate).ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0000", false)] // equal
    [InlineData("2000-01-01T00:00:00.0000", "2021-01-01T00:00:00.0000", true)] // compared date has higher year
    [InlineData("2000-01-01T00:00:00.0000", "1999-01-01T00:00:00.0000", false)] // compared date has lower year
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0001", true)] // compared date has higher millisecond
    [InlineData("2000-01-01T00:00:00.0001", "2000-01-01T00:00:00.0000", false)] // compared date has lower millisecond
    public void LessThanOperator(string mainDate, string leftDate, bool expectedResult)
    {
        var parsedLeftDate = new ExhaustionDate(DateTime.Parse(mainDate));
        var parsedRightDate = new ExhaustionDate(DateTime.Parse(leftDate));

        (parsedLeftDate < parsedRightDate).ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0000", true)] // equal
    [InlineData("2000-01-01T00:00:00.0000", "2021-01-01T00:00:00.0000", true)] // compared date has higher year
    [InlineData("2000-01-01T00:00:00.0000", "1999-01-01T00:00:00.0000", false)] // compared date has lower year
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0001", true)] // compared date has higher millisecond
    [InlineData("2000-01-01T00:00:00.0001", "2000-01-01T00:00:00.0000", false)] // compared date has lower millisecond
    public void LessThanOrEqualToOperator(string mainDate, string leftDate, bool expectedResult)
    {
        var parsedLeftDate = new ExhaustionDate(DateTime.Parse(mainDate));
        var parsedRightDate = new ExhaustionDate(DateTime.Parse(leftDate));

        (parsedLeftDate <= parsedRightDate).ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0000", false)] // equal
    [InlineData("2000-01-01T00:00:00.0000", "2021-01-01T00:00:00.0000", false)] // compared date has higher year
    [InlineData("2000-01-01T00:00:00.0000", "1999-01-01T00:00:00.0000", true)] // compared date has lower year
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0001", false)] // compared date has higher millisecond
    [InlineData("2000-01-01T00:00:00.0001", "2000-01-01T00:00:00.0000", true)] // compared date has lower millisecond
    public void GreaterThanOperator(string mainDate, string leftDate, bool expectedResult)
    {
        var parsedLeftDate = new ExhaustionDate(DateTime.Parse(mainDate));
        var parsedRightDate = new ExhaustionDate(DateTime.Parse(leftDate));

        (parsedLeftDate > parsedRightDate).ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0000", true)] // equal
    [InlineData("2000-01-01T00:00:00.0000", "2021-01-01T00:00:00.0000", false)] // compared date has higher year
    [InlineData("2000-01-01T00:00:00.0000", "1999-01-01T00:00:00.0000", true)] // compared date has lower year
    [InlineData("2000-01-01T00:00:00.0000", "2000-01-01T00:00:00.0001", false)] // compared date has higher millisecond
    [InlineData("2000-01-01T00:00:00.0001", "2000-01-01T00:00:00.0000", true)] // compared date has lower millisecond
    public void GreaterThanOrEqualToOperator(string mainDate, string leftDate, bool expectedResult)
    {
        var parsedLeftDate = new ExhaustionDate(DateTime.Parse(mainDate));
        var parsedRightDate = new ExhaustionDate(DateTime.Parse(leftDate));

        (parsedLeftDate >= parsedRightDate).ShouldBe(expectedResult);
    }
}
