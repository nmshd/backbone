using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Onboarding.Types.Responses;

public class OnboardingResponse
{
    public string? HtmlContent { get; set; }
    public string? RedirectionUrl { get; set; }
    public required int ResponseCode { get; set; }
}
