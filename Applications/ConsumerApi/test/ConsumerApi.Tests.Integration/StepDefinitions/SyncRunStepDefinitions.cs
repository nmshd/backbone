using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class SyncRunStepDefinitions
{
    #region Constructor, Fields, Properties
    private readonly DatawalletContext _datawalletContext;
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public SyncRunStepDefinitions(DatawalletContext datawalletContext, IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _datawalletContext = datawalletContext;
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private ClientPool ClientPool => _identitiesContext.ClientPool;
    #endregion

    [When(@"([a-zA-Z0-9]+) sends a POST request to /SyncRuns endpoint with the SyncRunType ""([a-zA-Z]+)""")]
    public async Task WhenISendsAPostRequestToSyncRunsEndpointWithTheSyncRunType(string identityName, string syncRunTypeName)
    {
        var identityAddress = ClientPool.FirstForIdentity(identityName)!.IdentityData!.Address;
        var datawallet = _datawalletContext.CreateDatawalletResponses.Values.First(cdwr => cdwr.Owner == identityAddress);

        var syncRunType = syncRunTypeName switch
        {
            "DatawalletVersionUpgrade" => SyncRunType.DatawalletVersionUpgrade,
            "ExternalEventSync" => SyncRunType.ExternalEventSync,
            _ => throw new ArgumentOutOfRangeException(nameof(syncRunTypeName), syncRunTypeName, null)
        };

        _responseContext.WhenResponse = _responseContext.StartSyncRunResponse = await ClientPool.FirstForIdentity(identityName)!.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = syncRunType }, datawallet.Version);
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to /SyncRuns endpoint with the SyncRunType ""([a-zA-Z]+)""")]
    public async Task WhenIdentitySendsAPutRequestToSyncRunsEndpointWithTheSyncRunType(string identityName, string syncRunTypeName)
    {
        var identityAddress = ClientPool.FirstForIdentity(identityName)!.IdentityData!.Address;
        var datawallet = _datawalletContext.CreateDatawalletResponses.Values.First(cdwr => cdwr.Owner == identityAddress);

        var syncRunType = syncRunTypeName switch
        {
            "DatawalletVersionUpgrade" => SyncRunType.DatawalletVersionUpgrade,
            "ExternalEventSync" => SyncRunType.ExternalEventSync,
            _ => throw new ArgumentOutOfRangeException(nameof(syncRunTypeName), syncRunTypeName, null)
        };

        _responseContext.StartSyncRunResponse = await ClientPool.FirstForIdentity(identityName)!.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = syncRunType }, datawallet.Version);

        var syncRunId = _responseContext.StartSyncRunResponse.Result!.SyncRun.Id;

        _responseContext.WhenResponse = _responseContext.FinalizeDatawalletVersionUpgradeResponse =
            await ClientPool.FirstForIdentity(identityName)!.SyncRuns.FinalizeDatawalletVersionUpgrade(syncRunId, new FinalizeDatawalletVersionUpgradeRequest
            {
                NewDatawalletVersion = 1,
                DatawalletModifications = []
            });
    }
}
