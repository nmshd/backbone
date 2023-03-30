using Challenges.API.Tests.Integration.API;
using Devices.API.Tests.Integration.Models;
using Microsoft.Extensions.Options;
    
using static Challenges.API.Tests.Integration.Configuration.Settings;

namespace Devices.API.Tests.Integration.StepDefinitions
{
    [Binding]
    public class GetIdentityListStepDefinitions
    {

        private readonly RequestConfiguration _requestConfiguration;
        private readonly IdentitiesApi _identitiesApi;
        private HttpResponse<List<IdentityDTO>>? _identitiesResponse;

        public GetIdentityListStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, IdentitiesApi identitiesApi)
        {
            _identitiesApi = identitiesApi;
            _requestConfiguration = new RequestConfiguration();
        }

   
        [When(@"a GET request is sent to the Identities/ endpoint")]
        public async Task WhenAGETRequestIsSentToTheIdentitiesEndpointAsync()
        {
            _identitiesResponse = await _identitiesApi.GetIdentitiesList(_requestConfiguration);
        }

        [Then(@"the response status code is (\d*) \((?:[a-z]|[A-Z]|\s)+\)")]
        public void ThenTheResponseStatusCodeIsOK(int code)
        {
            _identitiesResponse.Should().NotBeNull();
            ((int)_identitiesResponse!.StatusCode).Should().Be(code);
        }

        [Then(@"the response contains a list")]
        public void ThenTheResponseContainsAList()
        {
            _identitiesResponse.Should().NotBeNull();
            _identitiesResponse!.Data.Should().NotBeEmpty();
        }
    }
}
