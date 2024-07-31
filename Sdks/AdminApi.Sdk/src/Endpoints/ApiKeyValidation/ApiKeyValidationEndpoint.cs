﻿using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation;

public class ApiKeyValidationEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<ValidateApiKeyResponse>> ValidateApiKeyUnauthenticated(ValidateApiKeyRequest? request)
    {
        return await _client.PostUnauthenticated<ValidateApiKeyResponse>($"api/{API_VERSION}/ValidateApiKey", request);
    }
}
