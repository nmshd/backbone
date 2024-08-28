using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

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
    public ApiResponse<CanEstablishRelationshipResponse>? CanEstablishRelationshipResponse { get; set; }
    public ApiResponse<PushDatawalletModificationsResponse>? PushDatawalletModificationResponse { get; set; }
    public ApiResponse<CreateTokenResponse>? CreateTokenResponse { get; set; }
    public ApiResponse<EmptyResponse>? CreateTokenAnonymously { get; set; }
    public ApiResponse<ListTokensResponse>? ListTokensResponse { get; set; }
    public ApiResponse<Token>? GetTokenResponse { get; set; }

    public IResponse? WhenResponse { get; set; }
}
