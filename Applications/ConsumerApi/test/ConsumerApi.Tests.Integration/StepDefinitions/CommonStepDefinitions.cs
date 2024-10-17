using FluentAssertions.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class CommonStepDefinitions
{
    #region Given

    [Given(@"(\d+) second\(s\) have passed")]
    public async Task GivenXSecondsHavePassed(int numberOfSeconds)
    {
        await Task.Delay(numberOfSeconds.Seconds());
    }

    #endregion
}
