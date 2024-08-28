using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class DatawalletStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public DatawalletStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Datawallet/Modifications endpoint with a DatawalletModification in the payload")]
    public async Task WhenIdentitySendsAPostRequestToDatawalletModificationsEndpointWithADatawalletModificationInThePayload(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var pushDatawalletModificationsRequestItem = new PushDatawalletModificationsRequestItem
        {
            Type = "Create",
            ObjectIdentifier = "Y",
            Collection = "x",
            DatawalletVersion = 1
        };

        _responseContext.WhenResponse = await client.Datawallet.PushDatawalletModifications(
            new PushDatawalletModificationsRequest
            {
                LocalIndex = 0,
                Modifications = [pushDatawalletModificationsRequestItem]
            }, 1);
    }

    #endregion
}
