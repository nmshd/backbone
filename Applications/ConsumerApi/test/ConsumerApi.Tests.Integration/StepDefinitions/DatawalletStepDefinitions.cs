using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class DatawalletStepDefinitions
{
    #region Constructor, Fields, Properties
    private readonly DatawalletContext _datawalletContext;
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public DatawalletStepDefinitions(DatawalletContext datawalletContext, IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _datawalletContext = datawalletContext;
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private ClientPool ClientPool => _identitiesContext.ClientPool;
    #endregion

    #region Given
    [Given(@"a Datawallet ([a-zA-Z0-9]+) with DatawalletVersion set to ([0-9]+) created by ([a-zA-Z0-9]+)")]
    public async Task GivenADatawalletWithDatawalletVersionSetToCreatedByIdentity(string datawalletName, ushort datawalletVersion, string identityName)
    {
        _responseContext.CreateDatawalletResponse = await ClientPool.FirstForIdentity(identityName)!.Datawallet.CreateDatawallet(datawalletVersion);
        _datawalletContext.CreateDatawalletResponses[datawalletName] = _responseContext.CreateDatawalletResponse.Result!;
    }
    #endregion

    #region When
    [When(@"([a-zA-Z0-9]+) sends a POST request to /Datawallet endpoint with the following header\:")]
    public async Task WhenIdentitySendsAPostRequestToDatawalletEndpointWithTheFollowingHeader(string identityName, Table table)
    {
        var version = table.Rows[0].TryGetValue("Value", out var value) ? Convert.ToUInt16(value) : 0;
        _responseContext.WhenResponse = _responseContext.CreateDatawalletResponse = await ClientPool.FirstForIdentity(identityName)!.Datawallet.CreateDatawallet(version);
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to /Datawallet/Modifications endpoint with a DatawalletModification in the payload")]
    public async Task WhenIdentitySendsAPostRequestToDatawalletModificationsEndpointWithADatawalletModificationInThePayload(string identityName)
    {
        var identityAddress = ClientPool.FirstForIdentity(identityName)!.IdentityData!.Address;
        var datawallet = _datawalletContext.CreateDatawalletResponses.Values.First(cdwr => cdwr.Owner == identityAddress);

        var pushDatawalletModificationsRequestItem = new PushDatawalletModificationsRequestItem
        {
            Type = "Create",
            ObjectIdentifier = "Y",
            Collection = "x",
            DatawalletVersion = datawallet.Version
        };

        _responseContext.WhenResponse = _responseContext.PushDatawalletModificationResponse = await ClientPool.FirstForIdentity(identityName)!.Datawallet.PushDatawalletModifications(new PushDatawalletModificationsRequest
        {
            LocalIndex = 0,
            Modifications = [pushDatawalletModificationsRequestItem]
        }, datawallet.Version);
    }
    #endregion
}

public class DatawalletContext
{
    public Dictionary<string, CreateDatawalletResponse> CreateDatawalletResponses = new();
}
