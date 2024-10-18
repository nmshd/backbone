using Backbone.Tooling.Extensions;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
internal class CommonStepDefinitions
{
    [Given(@"(\d+) second\(s\) have passed")]
    public async Task GivenXSecondsHavePassed(int numberOfSeconds)
    {
        await Task.Delay(numberOfSeconds.Seconds());
    }
}
