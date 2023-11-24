using System.Net;
using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto.Abstractions;
using Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "DELETE Device")]
internal class DevicesStepDefinitions : BaseStepDefinitions
{
    private readonly DevicesApi _devicesApi;
    private HttpResponse? _response;
    private readonly string _deviceId;

    public DevicesStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, DevicesApi devicesApi, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi) : 
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi)
    {
        _devicesApi = devicesApi;
        _deviceId = string.Empty;
    }

    [Given(@"an owner of a Device d")]
    public void GivenAnOwnerOfADeviceD()
    {
        throw new PendingStepException();
    }

    [When(@"a DELETE request is sent to the Device/\{id} endpoint with ""?(.*?)""?")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWith(string id)
    {
        switch (id)
        {
            case "d.Id":
                id = _deviceId;
                break;
            case "a valid Id":
                id = "DVCnMMVu0h9T5oYHTI4z";
                break;
        }

        _response = await _devicesApi.DeleteUnOnboardedDevice($"Delete/{id}", _requestConfiguration, id);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int statusCode)
    {
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

}
