using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
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
    private ApiResponse<CreateFileResponse>? FileUploadResponse => _responseContext.FileUploadResponse;
    private ApiResponse<CreateIdentityResponse>? CreateIdentityResponse => _responseContext.CreateIdentityResponse;
    private ApiResponse<StartDeletionProcessResponse>? StartDeletionProcessResponse => _responseContext.StartDeletionProcessResponse;
    private ApiResponse<CancelDeletionProcessResponse>? CancelDeletionProcessResponse => _responseContext.CancelDeletionProcessResponse;
    private ApiResponse<SendMessageResponse>? SendMessageResponse => _responseContext.SendMessageResponse;
    private ApiResponse<UpdateDeviceRegistrationResponse>? UpdateDeviceRegistrationResponse => _responseContext.UpdateDeviceRegistrationResponse;
    private ApiResponse<RelationshipMetadata>? CreateRelationshipResponse => _responseContext.CreateRelationshipResponse;
    private ApiResponse<RelationshipMetadata>? AcceptRelationshipResponse => _responseContext.AcceptRelationshipResponse;
    private ApiResponse<RelationshipMetadata>? RejectRelationshipResponse => _responseContext.RejectRelationshipResponse;
    private ApiResponse<RelationshipMetadata>? RevokeRelationshipResponse => _responseContext.RevokeRelationshipResponse;

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

    [Then(@"the response contains a (.*)")]
    public async Task ThenTheResponseContains(string responseType)
    {
        if (responseType == "Challenge")
            await CheckResponse(ChallengeResponse);
        else if (responseType == "Device")
            await CheckResponse(RegisterDeviceResponse);
        else if (responseType == "CreateIdentityResponse")
            await CheckResponse(CreateIdentityResponse);
        else if (responseType == "StartDeletionProcessResponse")
            await CheckResponse(StartDeletionProcessResponse);
        else if (responseType == "CancelDeletionProcessResponse")
            await CheckResponse(CancelDeletionProcessResponse);
        else if (responseType == "FileUploadResponse")
            await CheckResponse(FileUploadResponse);
        else if (responseType == "SendMessageResponse")
            await CheckResponse(SendMessageResponse);
        else if (responseType == "UpdateDeviceRegistrationResponse")
            await CheckResponse(UpdateDeviceRegistrationResponse);
        else if (responseType == "CreateRelationshipTemplateResponse")
            await CheckResponse(_responseContext.CreateRelationshipTemplateResponse);
        else if (responseType == "Relationship")
            await CheckResponse(_responseContext.TerminateRelationshipResponse);
        else if (responseType == "RelationshipMetadata")
            await CheckRelationshipMetadata();
    }

    private static async Task CheckResponse<T>(ApiResponse<T>? response) where T : class
    {
        response!.Result.Should().NotBeNull();
        response.Should().BeASuccess();
        await response.Should().ComplyWithSchema();
    }

    #region Challenges
    [Then(@"the Challenge has a valid expiration date")]
    public void ThenTheChallengeHasAValidExpirationDate()
    {
        ChallengeResponse!.Result!.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
    #endregion

    #region Identities
    [Then(@"the response status is '([^']*)'")]
    public void ThenTheResponseStatusIs(string deletionProcessStatus)
    {
        CancelDeletionProcessResponse!.Result.Should().NotBeNull();
        CancelDeletionProcessResponse.Should().BeASuccess();
        CancelDeletionProcessResponse.Result!.Status.Should().Be(deletionProcessStatus);
    }
    #endregion

    #region Messages
    [Then(@"the error contains a list of Identities to be deleted that includes ([a-zA-Z0-9]+)")]
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

    #region PnsRegistrations
    [Then("the response contains the push identifier for the device")]
    public void ThenTheResponseContainsThePushIdentifierForTheDevice()
    {
        UpdateDeviceRegistrationResponse!.Result!.DevicePushIdentifier.Should().NotBeNullOrEmpty();
    }
    #endregion

    #region Relationships
    private async Task CheckRelationshipMetadata()
    {
        if (CreateRelationshipResponse != null)
            await CheckResponse(CreateRelationshipResponse);

        if (AcceptRelationshipResponse != null)
            await CheckResponse(AcceptRelationshipResponse);

        if (RejectRelationshipResponse != null)
            await CheckResponse(RejectRelationshipResponse);

        if (RevokeRelationshipResponse != null)
            await CheckResponse(RevokeRelationshipResponse);
    }
    #endregion
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
    public ApiResponse<CancelDeletionProcessResponse>? CancelDeletionProcessResponse { get; set; }
    public ApiResponse<ListMessagesResponse>? GetMessagesResponse { get; set; }
    public ApiResponse<SendMessageResponse>? SendMessageResponse { get; set; }
    public ApiResponse<UpdateDeviceRegistrationResponse>? UpdateDeviceRegistrationResponse { get; set; }
    public ApiResponse<CreateRelationshipTemplateResponse>? CreateRelationshipTemplateResponse { get; set; }
    public ApiResponse<RelationshipMetadata>? CreateRelationshipResponse { get; set; }
    public ApiResponse<RelationshipMetadata>? AcceptRelationshipResponse { get; set; }
    public ApiResponse<RelationshipMetadata>? RejectRelationshipResponse { get; set; }
    public ApiResponse<RelationshipMetadata>? RevokeRelationshipResponse { get; set; }
    public ApiResponse<Relationship>? TerminateRelationshipResponse { get; set; }
    public ApiResponse<RelationshipMetadata>? DecomposeRelationshipResponse { get; set; }

    public IResponse? WhenResponse { get; set; }
}
