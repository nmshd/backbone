using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Onboarding;

public class OnboardingEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<EmptyResponse>> RequestOnboardingInformation()
    {
        return await _client.GetUnauthenticated<EmptyResponse>("Onboarding");
    }
}
