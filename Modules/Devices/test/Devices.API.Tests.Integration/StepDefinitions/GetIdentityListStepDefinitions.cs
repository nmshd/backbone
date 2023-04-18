using Devices.API.Tests.Integration.API;
using Devices.API.Tests.Integration.Models;

namespace Devices.API.Tests.Integration.StepDefinitions
{
    [Binding]
    public class GetIdentityListStepDefinitions
    {
        private readonly ResponseData _responseData;
        private readonly RequestConfiguration _requestConfiguration;
        private readonly IdentitiesApi _identitiesApi;
        private HttpResponse<ListIdentitiesResponse>? _identitiesResponse;
        private List<IdentitySummaryDTO>? _identitiesList;

        public GetIdentityListStepDefinitions(IdentitiesApi identitiesApi, ResponseData responseData)
        {
            _responseData = responseData;
            _identitiesApi = identitiesApi;
            _requestConfiguration = new RequestConfiguration();
        }

   
        [When(@"a GET request is sent to the Identities/ endpoint")]
        public async Task WhenAGETRequestIsSentToTheIdentitiesEndpointAsync()
        {
            _identitiesResponse = await _identitiesApi.GetIdentities(_requestConfiguration);
            _identitiesResponse.Should().NotBeNull();
            _identitiesResponse!.Data.Should().NotBeNull();
            _identitiesList = _identitiesResponse!.Data!.Result;

            _responseData.ResponseStatus = _identitiesResponse!.StatusCode;
        }

        [Then(@"the response contains a paginated list of Identities")]
        public void ThenTheResponseContainsAList()
        {
            _identitiesList.Should().NotBeNull();
            _identitiesList!.Should().NotBeEmpty();
        }
    }
}
