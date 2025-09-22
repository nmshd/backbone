using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class IdentityDeletionProcessesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<CancelDeletionProcessResponse>? _cancelDeletionProcessResponse;

    public IdentityDeletionProcessesStepDefinitions(IdentitiesContext identitiesContext, ResponseContext responseContext, ClientPool clientPool)
    {
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given("no active Identity Deletion Process for i exists")]
    public void GivenNoActiveDeletionProcessForIdentityExists()
    {
        // this step is only for readability; since no deletion process exists, we don't need to do anything
    }

    [Given($"an active deletion process {RegexFor.SINGLE_THING} for {RegexFor.SINGLE_THING} exists")]
    public async Task GivenAnActiveDeletionProcessDpForTheIdentityExists(string deletionProcessName, string identityName)
    {
        var deletionProcess = await _clientPool.FirstForIdentityName(identityName).Identities.StartDeletionProcess();
        _identitiesContext.StartDeletionProcessResponses[deletionProcessName] = deletionProcess.Result!;
    }

    [Given($"an active deletion process for {RegexFor.SINGLE_THING} exists")]
    public async Task GivenAnActiveDeletionProcessForTheIdentityExists(string identityName)
    {
        await _clientPool.FirstForIdentityName(identityName).Identities.StartDeletionProcess();
    }

    [Given($"{RegexFor.SINGLE_THING} is in status \"ToBeDeleted\"")]
    public async Task GivenIdentityIsToBeDeleted(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        await client.Identities.StartDeletionProcess();
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Identities/Self/DeletionProcesses endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheIdentitiesSelfDeletionProcessesEndpoint(string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = await client.Identities.StartDeletionProcess();
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Identities/Self/DeletionProcesses/{RegexFor.SINGLE_THING}.Id endpoint")]
    public async Task WhenIdentitySendsAPutRequestToTheIdentitiesSelfDeletionProcessesIdEndpoint(string identityName, string deletionProcessName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _responseContext.WhenResponse = _cancelDeletionProcessResponse =
            await client.Identities.CancelDeletionProcess(_identitiesContext.StartDeletionProcessResponses[deletionProcessName].Id);
    }

    #endregion

    #region Then

    [Then($"the new status of {RegexFor.SINGLE_THING} is '([a-zA-Z]+)'")]
    public void ThenTheNewStatusOfTheDeletionProcessIs(string deletionProcessName, string newDeletionProcessStatus)
    {
        _cancelDeletionProcessResponse!.Result!.Status.ShouldBe(newDeletionProcessStatus);
    }

    #endregion
}
