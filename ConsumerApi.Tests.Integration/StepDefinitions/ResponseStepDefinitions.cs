using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class ResponseStepDefinitions
{
    private readonly IdentitiesContext _identitiesContext;
    private readonly MessagesContext _messagesContext;
    private readonly ResponseContext _responseContext;

    public ResponseStepDefinitions(IdentitiesContext identitiesContext, MessagesContext messagesContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _messagesContext = messagesContext;
        _responseContext = responseContext;
    }

    private ApiResponse<Challenge>? ChallengeResponse => _responseContext.ChallengeResponse;
    private ApiResponse<RegisterDeviceResponse>? RegisterDeviceResponse => _responseContext.RegisterDeviceResponse;
    private ApiResponse<EmptyResponse>? UpdateDeviceResponse => _responseContext.UpdateDeviceResponse;
    private ApiResponse<EmptyResponse>? DeleteDeviceResponse => _responseContext.DeleteDeviceResponse;
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

    #region Devices
    [Then(@"the response contains a Device")]
    public async Task ThenTheResponseContainsADevice()
    {
        RegisterDeviceResponse!.Result.Should().NotBeNull();
        RegisterDeviceResponse.Should().BeASuccess();
        await RegisterDeviceResponse.Should().ComplyWithSchema();
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

    [Then(@"the error contains a list of Identities to be deleted that includes (.+)")]
    public void ThenTheErrorContainsAListOfIdentitiesToBeDeletedThatIncludesIdentity(string identityName)
    {
        var errorData = _responseContext.SendMessageResponse!.Error!.Data?.As<PeersToBeDeletedErrorData>();
        errorData.Should().NotBeNull();
        errorData!.PeersToBeDeleted.Contains(_identitiesContext.Identities[identityName].IdentityData!.Address).Should().BeTrue();
    }

    [Then(@"the response contains the Messages ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsTheMessagesMAndM(string message1Name, string message2Name)
    {
        var message1 = _messagesContext.Messages[message1Name];
        var message2 = _messagesContext.Messages[message2Name];

        ThrowIfNull(_responseContext.GetMessagesResponse);

        _responseContext.GetMessagesResponse.Result.Should().Contain(m => m.Id == message1.Id);
        _responseContext.GetMessagesResponse.Result.Should().Contain(m => m.Id == message2.Id);
    }

    [Then(@"the response contains the Message ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsTheMessageM(string messageName)
    {
        var message = _messagesContext.Messages[messageName];

        ThrowIfNull(_responseContext.GetMessagesResponse);

        _responseContext.GetMessagesResponse.Result.Should().Contain(m => m.Id == message.Id);
    }

    [Then(@"the response does not contain the Message ([a-zA-Z0-9]+)")]
    public void ThenTheResponseDoesNotContainTheMessageM(string messageName)
    {
        var message = _messagesContext.Messages[messageName];

        ThrowIfNull(_responseContext.GetMessagesResponse);

        _responseContext.GetMessagesResponse.Result.Should().NotContain(m => m.Id == message.Id);
    }
    #endregion

    private void AssertExpirationDateIsInFuture()
    {
        _responseContext.ChallengeResponse!.Result!.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}

public class ResponseContext
{
    public ApiResponse<Challenge>? ChallengeResponse { get; set; }
    public ApiResponse<RegisterDeviceResponse>? RegisterDeviceResponse { get; set; }
    public ApiResponse<EmptyResponse>? UpdateDeviceResponse { get; set; }
    public ApiResponse<EmptyResponse>? DeleteDeviceResponse { get; set; }
    public ApiResponse<CreateFileResponse>? FileUploadResponse { get; set; }
    public ApiResponse<CreateIdentityResponse>? CreateIdentityResponse { get; set; }
    public ApiResponse<StartDeletionProcessResponse>? StartDeletionProcessResponse { get; set; }
    public ApiResponse<ListMessagesResponse>? GetMessagesResponse { get; set; }
    public ApiResponse<SendMessageResponse>? SendMessageResponse { get; set; }

    public ApiResponse<Relationship>? TerminateRelationshipResponse { get; set; }
    public ApiResponse<RelationshipMetadata>? DecomposeRelationshipResponse { get; set; }

    public IResponse? WhenResponse { get; set; }
}
