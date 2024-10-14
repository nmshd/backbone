using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using FluentAssertions.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class SynchronizationStepDefinitions
{
    private readonly ClientPool _clientPool;
    private readonly Dictionary<string, StartSyncRunResponse> _startSyncRunResponses = new();
    private readonly ResponseContext _responseContext;
    private readonly MessagesContext _messagesContext;
    private readonly RelationshipsContext _relationshipsContext;
    private ApiResponse<ListExternalEventsResponse>? _listExternalEventsOfSyncRunResponse;

    public SynchronizationStepDefinitions(ResponseContext responseContext, MessagesContext messagesContext, RelationshipsContext relationshipsContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _messagesContext = messagesContext;
        _relationshipsContext = relationshipsContext;
        _clientPool = clientPool;
    }

    #region Given

    [Given($"a sync run {RegexFor.SINGLE_THING} started by {RegexFor.SINGLE_THING}")]
    public async Task GivenASyncRunStartedBy(string syncRunName, string identityName)
    {
        await Task.Delay(1.Seconds());

        var client = _clientPool.FirstForIdentityName(identityName);

        var startSyncRunResponse = await client.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.ExternalEventSync }, 1);

        _startSyncRunResponses.Add(syncRunName, startSyncRunResponse.Result!);
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /SyncRuns/{RegexFor.SINGLE_THING}.id/ExternalEvents endpoint")]
    public async Task WhenISendsAGETRequestToTheSyncRunsSrIdExternalEventsEndpoint(string identityName, string syncRunName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        var syncRunId = _startSyncRunResponses[syncRunName].SyncRun.Id;

        _responseContext.WhenResponse = _listExternalEventsOfSyncRunResponse = await client.SyncRuns.ListExternalEventsOfSyncRun(syncRunId);
    }

    #endregion

    #region Then

    [Then($"the response does not contain an external event for {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseDoesNotContainAnExternalEventForM(string messageName)
    {
        _listExternalEventsOfSyncRunResponse!.Result.Should().NotBeEmpty();
        _listExternalEventsOfSyncRunResponse.Result.Should().NotContain(e => e.Type == "MessageReceived");
    }

    [Then($"the response contains an external event for {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseContainsAnExternalEventForM(string messageName)
    {
        var message = _messagesContext.Messages[messageName];
        _listExternalEventsOfSyncRunResponse!.Result.Should().NotBeEmpty();
        _listExternalEventsOfSyncRunResponse.Result.Should().ContainSingle(
            e => e.Type == "MessageReceived"
        );
        var messageReceivedExternalEvent = _listExternalEventsOfSyncRunResponse.Result!.Single(e => e.Type == "MessageReceived");
        messageReceivedExternalEvent.Payload["id"].GetString().Should().Be(message.Id);
    }

    #endregion
}
