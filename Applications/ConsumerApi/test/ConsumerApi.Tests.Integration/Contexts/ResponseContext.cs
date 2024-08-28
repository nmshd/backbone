using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class ResponseContext
{
    public ApiResponse<Challenge>? ChallengeResponse { get; set; }
    public ApiResponse<StartDeletionProcessResponse>? StartDeletionProcessResponse { get; set; }
    public ApiResponse<CancelDeletionProcessResponse>? CancelDeletionProcessResponse { get; set; }
    public ApiResponse<ListMessagesResponse>? GetMessagesResponse { get; set; }
    public ApiResponse<SendMessageResponse>? SendMessageResponse { get; set; }
    public ApiResponse<UpdateDeviceRegistrationResponse>? UpdateDeviceRegistrationResponse { get; set; }
    public ApiResponse<RelationshipMetadata>? DecomposeRelationshipResponse { get; set; }
    public ApiResponse<CanEstablishRelationshipResponse>? CanEstablishRelationshipResponse { get; set; }
    public ApiResponse<ListTokensResponse>? ListTokensResponse { get; set; }

    public IResponse? WhenResponse { get; set; }
}
