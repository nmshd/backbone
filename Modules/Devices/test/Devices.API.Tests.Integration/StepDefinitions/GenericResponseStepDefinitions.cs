using Devices.API.Tests.Integration.Models;

namespace Devices.API.Tests.Integration.StepDefinitions
{
    [Binding]
    public class GenericResponseStepDefinitions
    {
        private readonly ResponseData _responseData;

        public GenericResponseStepDefinitions(ResponseData responseData)
        {
            _responseData = responseData;
        }

        [Then(@"the response status code is (\d\d\d) \((?:[a-z]|[A-Z]|\s)+\)")]
        public void ThenTheResponseStatusCodeIs(int code)
        {
            var responseStatus = _responseData.ResponseStatus;

            ((int)responseStatus).Should().Be(code);
        }
    }
}
