﻿using AdminUi.Tests.Integration.API;
using AdminUi.Tests.Integration.Extensions;
using AdminUi.Tests.Integration.Models;

namespace AdminUi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST IndividualQuota")]
[Scope(Feature = "DELETE IndividualQuota")]
public class IndividualQuotaStepDefinitions : BaseStepDefinitions
{
    private readonly IdentitiesApi _identitiesApi;
    private string _identityAddress;
    private string _quotaId;
    private HttpResponse<IndividualQuotaDTO>? _response;

    public IndividualQuotaStepDefinitions(IdentitiesApi identitiesApi)
    {
        _identitiesApi = identitiesApi;
        _identityAddress = string.Empty;
        _quotaId = string.Empty;
    }

    [Given(@"an Identity i")]
    public void GivenAnIdentity()
    {
        _identityAddress = "id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm";
    }

    [Given(@"an Identity i with an IndividualQuota q")]
    public async Task GivenAnIdentityIWithAnIndividualQuotaQ()
    {
        _identityAddress = "id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm";
        var createIndividualQuotaRequest = new CreateIndividualQuotaRequest()
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        };
        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createIndividualQuotaRequest);

        var response = await _identitiesApi.CreateIndividualQuota(requestConfiguration, _identityAddress);
        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(201);
        _quotaId = response.Content.Result!.Id;
    }

    [When(@"a DELETE request is sent to the /Identities/{i.address}/Quotas/{q.id} endpoint")]
    public async Task WhenADeleteRequestIsSentToTheDeleteIndividualQuotaEndpoint()
    {
        _response = await _identitiesApi.DeleteIndividualQuota(_identityAddress, _quotaId, _requestConfiguration);
        _response.Should().NotBeNull();
    }

    [When(@"a DELETE request is sent to the /Identities/{t.address}/Quotas/inexistentQuotaId endpoint")]
    public async Task WhenADeleteRequestIsSentToTheDeleteIndividualQuotaEndpointWithAnInexistentQuotaId()
    {
        _response = await _identitiesApi.DeleteIndividualQuota(_identityAddress, "some-inexistent-quota-id", _requestConfiguration);
        _response.Should().NotBeNull();
    }

    [When(@"a POST request is sent to the /Identity/{i.id}/Quotas endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheCreateIndividualQuotaEndpoint()
    {
        var createIndividualQuotaRequest = new CreateIndividualQuotaRequest()
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createIndividualQuotaRequest);

        _response = await _identitiesApi.CreateIndividualQuota(requestConfiguration, _identityAddress);
    }

    [When(@"a POST request is sent to the /Identity/{address}/Quotas endpoint with an inexistent identity address")]
    public async Task WhenAPOSTRequestIsSentToTheCreateIndividualQuotaEndpointWithAnInexistentIdentityAddress()
    {
        var createIndividualQuotaRequest = new CreateIndividualQuotaRequest()
        {
            MetricKey = "NumberOfSentMessages",
            Max = 2,
            Period = "Week"
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createIndividualQuotaRequest);

        _response = await _identitiesApi.CreateIndividualQuota(requestConfiguration, "some-inexistent-identity-address");
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the response contains an IndividualQuota")]
    public void ThenTheResponseContainsAnIndividualQuota()
    {
        _response!.AssertHasValue();
        _response!.AssertStatusCodeIsSuccess();
        _response!.AssertContentTypeIs("application/json");
        _response!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response!.Content.Error.Should().NotBeNull();
        _response.Content.Error!.Code.Should().Be(errorCode);
    }
}
