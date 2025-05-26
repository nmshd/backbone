using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class SynchronizationStepDefinitions
{
    private readonly ClientPool _clientPool;
    private readonly Dictionary<string, StartSyncRunResponse> _startSyncRunResponses = new();
    private readonly ResponseContext _responseContext;
    private readonly MessagesContext _messagesContext;
    private readonly RelationshipTemplatesContext _relationshipTemplatesContext;
    private readonly FilesContext _filesContext;
    private ApiResponse<ListExternalEventsResponse>? _listExternalEventsOfSyncRunResponse;

    public SynchronizationStepDefinitions(ResponseContext responseContext, MessagesContext messagesContext, ClientPool clientPool, RelationshipTemplatesContext relationshipTemplatesContext,
        FilesContext filesContext)
    {
        _responseContext = responseContext;
        _messagesContext = messagesContext;
        _clientPool = clientPool;
        _relationshipTemplatesContext = relationshipTemplatesContext;
        _filesContext = filesContext;
    }

    #region Given

    [Given($"a sync run {RegexFor.SINGLE_THING} started by {RegexFor.SINGLE_THING}")]
    public async Task GivenASyncRunStartedBy(string syncRunName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var startSyncRunResponse = await client.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.ExternalEventSync }, 1);

        _startSyncRunResponses.Add(syncRunName, startSyncRunResponse.Result!);
    }

    #endregion

    #region When

    [When($"^{RegexFor.SINGLE_THING} sends a GET request to the /SyncRuns/{RegexFor.SINGLE_THING}.id/ExternalEvents endpoint$")]
    public async Task WhenISendsAGETRequestToTheSyncRunsSrIdExternalEventsEndpoint(string identityName, string syncRunName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var syncRunId = _startSyncRunResponses[syncRunName].SyncRun.Id;

        _responseContext.WhenResponse = _listExternalEventsOfSyncRunResponse = await client.SyncRuns.ListExternalEventsOfSyncRun(syncRunId);
    }

    #endregion

    #region Then

    [Then($"the response does not contain an external event for the Message {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseDoesNotContainAnExternalEventForM(string _)
    {
        _listExternalEventsOfSyncRunResponse!.Result.ShouldNotBeEmpty();
        _listExternalEventsOfSyncRunResponse.Result.ShouldNotContain(e => e.Type == "MessageReceived");
    }

    [Then($"the response contains an external event for the Message {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseContainsAnExternalEventForM(string messageName)
    {
        var message = _messagesContext.Messages[messageName];
        _listExternalEventsOfSyncRunResponse!.Result.ShouldNotBeEmpty();
        _listExternalEventsOfSyncRunResponse.Result.ShouldContainSingle(e => e.Type == "MessageReceived");
        var messageReceivedExternalEvent = _listExternalEventsOfSyncRunResponse.Result!.Single(e => e.Type == "MessageReceived");
        messageReceivedExternalEvent.Payload["id"].GetString().ShouldBe(message.Id);
    }

    [Then($@"{RegexFor.SINGLE_THING} receives an ExternalEvent {RegexFor.SINGLE_THING} of type PeerFeatureFlagsChanged which contains the address of {RegexFor.SINGLE_THING}")]
    public async Task ThenIReceivesAnExternalEventEOfTypePeerFeatureFlagsChanged(string notifiedIdentityName, string externalEventName, string peerName)
    {
        var client = _clientPool.FirstForIdentityName(notifiedIdentityName);
        var syncRunResponse = await client.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.ExternalEventSync }, 1);

        syncRunResponse.Result.ShouldNotBeNull();
        syncRunResponse.Result!.Status.ShouldBe("Created");
        var externalEvents = await client.SyncRuns.ListExternalEventsOfSyncRun(syncRunResponse.Result!.SyncRun.Id);

        var peerAddress = _clientPool.FirstForIdentityName(peerName).IdentityData!.Address;

        externalEvents.Result!.ShouldContain(e =>
            e.Type == "PeerFeatureFlagsChanged" &&
            e.Payload["peerAddress"].GetString() == peerAddress);
    }

    [Then($@"{RegexFor.SINGLE_THING} receives an ExternalEvent of type RelationshipTemplateAllocationsExhausted which contains the id of Relationship Template {RegexFor.SINGLE_THING}")]
    public async Task ThenIRecievesAnExternalEventOfTypeRelationshipTemplateAllocationsExhausted(string notifiedIdentityName, string relationshipTemplateName)
    {
        var client = _clientPool.FirstForIdentityName(notifiedIdentityName);
        var syncRunResponse = await client.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.ExternalEventSync }, 1);

        syncRunResponse.Result.ShouldNotBeNull();
        syncRunResponse.Result!.Status.ShouldBe("Created");
        var externalEvents = await client.SyncRuns.ListExternalEventsOfSyncRun(syncRunResponse.Result!.SyncRun.Id);

        var templateId = _relationshipTemplatesContext.CreateRelationshipTemplatesResponses[relationshipTemplateName].Id;

        externalEvents.Result!.ShouldContain(e =>
            e.Type == "RelationshipTemplateAllocationsExhausted" &&
            e.Payload["relationshipTemplateId"].GetString() == templateId);
    }

    [Then($@"{RegexFor.SINGLE_THING} receives an ExternalEvent of type FileOwnershipLockedEvent which contains the id of {RegexFor.SINGLE_THING}")]
    public async Task ThenIReceivesAnExternalEventOfTypeFileOwnershipLocked(string notifiedIdentityName, string fileName)
    {
        var client = _clientPool.FirstForIdentityName(notifiedIdentityName);
        var syncRunResponse = await client.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.ExternalEventSync }, 1);

        syncRunResponse.Result.Should().NotBeNull();
        syncRunResponse.Result!.Status.Should().Be("Created");
        var externalEvents = await client.SyncRuns.ListExternalEventsOfSyncRun(syncRunResponse.Result!.SyncRun.Id);

        var fileId = _filesContext.Files[fileName].Id;

        externalEvents.Result.Should().Contain(e =>
            e.Type == "FileOwnershipLocked" &&
            e.Payload["fileId"].GetString() == fileId);
    }

    #endregion
}
