using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class ResponseStepDefinitions
{
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public ResponseStepDefinitions(IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private ApiResponse<Challenge>? ChallengeResponse => _responseContext.ChallengeResponse;
    private ApiResponse<CreateFileResponse>? FileUploadResponse => _responseContext.FileUploadResponse;
    private ApiResponse<CreateIdentityResponse>? CreateIdentityResponse => _responseContext.CreateIdentityResponse;
    private ApiResponse<StartDeletionProcessResponse>? StartDeletionProcessResponse => _responseContext.StartDeletionProcessResponse;
    private ApiResponse<SendMessageResponse>? SendMessageResponse => _responseContext.SendMessageResponse;

    private IResponse? WhenResponse => _responseContext.WhenResponse;

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ThrowIfNull(WhenResponse);
        ((int)WhenResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentContainsAnErrorWithTheErrorCode(string errorCode)
    {
        WhenResponse!.Error.Should().NotBeNull();
        WhenResponse.Error!.Code.Should().Be(errorCode);
    }

    #region Challenges
    [Then("the response contains a Challenge")]
    public async Task ThenTheResponseContainsAChallenge()
    {
        ChallengeResponse!.Should().NotBeNull();
        ChallengeResponse!.Should().BeASuccess();
        ChallengeResponse!.ContentType.Should().Be("application/json");
        await ChallengeResponse.Should().ComplyWithSchema();
        AssertExpirationDateIsInFuture();
    }
    #endregion

    #region Identities
    [Then(@"the response contains a CreateIdentityResponse")]
    public async Task ThenTheResponseContainsACreateIdentityResponse()
    {
        CreateIdentityResponse!.Should().NotBeNull();
        CreateIdentityResponse!.Should().BeASuccess();
        await CreateIdentityResponse!.Should().ComplyWithSchema();
    }

    [Then(@"the response contains a Deletion Process")]
    public async Task ThenTheResponseContainsADeletionProcess()
    {
        StartDeletionProcessResponse!.Result.Should().NotBeNull();
        StartDeletionProcessResponse.Should().BeASuccess();
        await StartDeletionProcessResponse.Should().ComplyWithSchema();
    }
    #endregion

    #region Files
    [Then(@"the response contains a CreateFileResponse")]
    public async Task ThenTheResponseContainsACreateFileResponse()
    {
        FileUploadResponse!.Result.Should().NotBeNull();
        FileUploadResponse.Should().BeASuccess();
        await FileUploadResponse.Should().ComplyWithSchema();
    }
    #endregion

    #region Messages
    [Then("the response contains a SendMessageResponse")]
    public async Task ThenTheResponseContainsASendMessageResponse()
    {
        SendMessageResponse!.Result.Should().NotBeNull();
        SendMessageResponse.Should().BeASuccess();
        await SendMessageResponse.Should().ComplyWithSchema();
    }

    [Then(@"the response error contains a list of Identities to be deleted that includes (.+)")]
    public void ThenTheErrorContainsAListOfIdentitiesToBeDeletedThatIncludesIdentity(string identityName)
    {
        var errorData = _responseContext.SendMessageResponse!.Error!.Data?.As<PeersToBeDeletedErrorData>();
        errorData.Should().NotBeNull();
        errorData!.PeersToBeDeleted.Contains(_identitiesContext.Identities[identityName].IdentityData!.Address).Should().BeTrue();
    }
    #endregion

    private void AssertExpirationDateIsInFuture()
    {
        _responseContext.ChallengeResponse!.Result!.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}

public class ResponseContext
{
    public ApiResponse<Challenge>? ChallengeResponse { get; set; } = null!;
    public ApiResponse<CreateFileResponse>? FileUploadResponse { get; set; } = null!;
    public ApiResponse<CreateIdentityResponse>? CreateIdentityResponse { get; set; } = null!;
    public ApiResponse<StartDeletionProcessResponse>? StartDeletionProcessResponse { get; set; }
    public ApiResponse<SendMessageResponse>? SendMessageResponse { get; set; } = null!;

    public IResponse? WhenResponse { get; set; } = null!;
}
