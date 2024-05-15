﻿using Backbone.AdminApi.Sdk.Endpoints.Challenges.Types;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Challenges;

public class ChallengesEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<Challenge>> CreateChallenge()
    {
        return await _client.Post<Challenge>($"api/{API_VERSION}/Challenges");
    }
}
