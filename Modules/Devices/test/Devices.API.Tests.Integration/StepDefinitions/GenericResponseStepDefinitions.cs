namespace Devices.API.Tests.Integration.StepDefinitions
{
    [Binding]
    public class GenericResponseStepDefinitions
    {
        private ScenarioContext _scenarioContext;
        private object? _data;
        private object? _responseStatus;

        public GenericResponseStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Then(@"the response status code is (\d\d\d) \((?:[a-z]|[A-Z]|\s)+\)")]
        public void ThenTheResponseStatusCodeIs(int code)
        {
            _data = _scenarioContext["Data"];
            _responseStatus = _scenarioContext["ResponseStatus"];

            _data.Should().NotBeNull();
            ((int)_responseStatus).Should().Be(code);
        }
    }
}
