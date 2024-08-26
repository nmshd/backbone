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
    public async Task WhenIdentitySendsAPostRequestToSyncRunsEndpointWithTheSyncRunType(string identityName, string syncRunTypeName)
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

    [When(@"([a-zA-Z0-9]+) sends a PUT request to /SyncRuns endpoint with the SyncRunType ""DatawalletVersionUpgrade""")]
    public async Task WhenIdentitySendsAPutRequestToSyncRunsEndpointWithTheSyncRunTypeDatawalletVersionUpgrade(string identityName)
    {
        var identityAddress = ClientPool.FirstForIdentity(identityName)!.IdentityData!.Address;
        var datawallet = _datawalletContext.CreateDatawalletResponses.Values.First(cdwr => cdwr.Owner == identityAddress);

        _responseContext.StartSyncRunResponse = await ClientPool.FirstForIdentity(identityName)!.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.DatawalletVersionUpgrade }, datawallet.Version);

        var syncRunId = _responseContext.StartSyncRunResponse.Result!.SyncRun.Id;

        _responseContext.WhenResponse = _responseContext.FinalizeDatawalletVersionUpgradeResponse =
            await ClientPool.FirstForIdentity(identityName)!.SyncRuns.FinalizeDatawalletVersionUpgrade(syncRunId, new FinalizeDatawalletVersionUpgradeRequest
            {
                NewDatawalletVersion = 1,
                DatawalletModifications = []
            });
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to /SyncRuns endpoint with the SyncRunType ""ExternalEventSync""")]
    public async Task WhenIdentitySendsAPutRequestToSyncRunsEndpointWithTheSyncRunTypeExternalEventSync(string identityName)
    {
        var syncRunId = await GetExternalEventSyncRunId(identityName);

        _responseContext.WhenResponse = _responseContext.FinalizeExternalEventSyncResponse =
            await ClientPool.FirstForIdentity(identityName)!.SyncRuns.FinalizeExternalEventSync(syncRunId, new FinalizeExternalEventSyncRequest
            {
                ExternalEventResults = [],
                DatawalletModifications = []
            });
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to /SyncRuns/\{id}/RefreshExpirationTime endpoint")]
    public async Task WhenIdentitySendsAPutRequestToSyncRunsIdRefreshExpirationTimeEndpoint(string identityName)
    {
        var syncRunId = await GetExternalEventSyncRunId(identityName);

        _responseContext.WhenResponse = _responseContext.RefreshExpirationTimeResponse = await ClientPool.FirstForIdentity(identityName)!.SyncRuns.RefreshExpirationTime(syncRunId);
    }

    private async Task<string> GetExternalEventSyncRunId(string identityName)
    {
        var identityAddress = ClientPool.FirstForIdentity(identityName)!.IdentityData!.Address;
        var datawallet = _datawalletContext.CreateDatawalletResponses.Values.First(cdwr => cdwr.Owner == identityAddress);

        _responseContext.StartSyncRunResponse = await ClientPool.FirstForIdentity(identityName)!.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.ExternalEventSync }, datawallet.Version);

        return _responseContext.StartSyncRunResponse.Result!.SyncRun.Id;
    }
}
