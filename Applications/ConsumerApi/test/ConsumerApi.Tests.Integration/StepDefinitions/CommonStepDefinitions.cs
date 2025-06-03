namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class CommonStepDefinitions
{
    [Given(@"(\d+) second\(s\) have passed")]
    [When(@"(\d+) second\(s\) have passed")]
    public async Task GivenXSecondsHavePassed(int numberOfSeconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(numberOfSeconds));
    }
}
