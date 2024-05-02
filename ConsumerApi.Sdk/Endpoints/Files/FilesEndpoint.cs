using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Files;

public class FilesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<CreateFileResponse>> UploadFile(CreateFileRequest request) => await _client
        .Request<CreateFileResponse>(HttpMethod.Post, $"api/{API_VERSION}/Files")
        .Authenticate()
        .WithMultipartForm(request.ToMultipartContent())
        .Execute();

    public async Task<ApiResponse<List<FileMetadata>>> ListFileMetadata(PaginationFilter? pagination = null)
        => await _client.Get<List<FileMetadata>>($"api/{API_VERSION}/Files", null, pagination);

    public async Task<ApiResponse<List<FileMetadata>>> ListFileMetadata(List<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<List<FileMetadata>>(HttpMethod.Get, $"api/{API_VERSION}/Files")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ApiResponse<FileMetadata>> GetFileMetadata(string id) => await _client.Get<FileMetadata>($"api/{API_VERSION}/Files/{id}/metadata");

    public async Task<RawApiResponse> DownloadFile(string id) => await _client
        .Request<RawApiResponse>(HttpMethod.Get, $"api/{API_VERSION}/Files/{id}")
        .Authenticate()
        .ExecuteRaw();
}
