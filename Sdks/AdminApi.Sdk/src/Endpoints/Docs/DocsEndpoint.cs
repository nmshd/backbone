using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Docs;

public class DocsEndpoint
{
    private readonly HttpClient _httpClient;

    public DocsEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<string>> GetSwaggerUi()
    {
        return await GetStringContent("/docs/index.html");
    }

    public async Task<ApiResponse<string>> GetOpenApiSpecV1()
    {
        return await GetStringContent("/docs/v1/openapi.json");
    }

    private async Task<ApiResponse<string>> GetStringContent(string path)
    {
        var response = await _httpClient.GetAsync(path);

        var responseContent = await response.Content.ReadAsStringAsync();

        return new ApiResponse<string>
        {
            ContentType = response.Content.Headers.ContentType?.ToString(),
            Result = responseContent,
            Status = response.StatusCode,
            RawContent = responseContent
        };
    }
}
