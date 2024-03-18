using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation.Types;
using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation.Types.Responses;
using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.ApiKeyValidation;

public class ApiKeyValidationEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<ValidateApiKeyResponse>> ValidateApiKey(ValidateApiKeyRequest? request)
        => await _client.Post<ValidateApiKeyResponse>("ValidateApiKey", request);

    public async Task<AdminApiResponse<ValidateApiKeyResponse>> ValidateApiKeyUnauthenticated(ValidateApiKeyRequest? request)
        => await _client.PostUnauthenticated<ValidateApiKeyResponse>("ValidateApiKey", request);
}
