using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Files;

public class FilesEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<CreateFileResponse>> UploadFile(CreateFileRequest request)
    {
        return await _client
            .Request<CreateFileResponse>(HttpMethod.Post, $"api/{API_VERSION}/Files")
            .Authenticate()
            .WithMultipartForm(request.ToMultipartContent())
            .Execute();
    }

    public async Task<ApiResponse<List<FileMetadata>>> ListFileMetadata(PaginationFilter? pagination = null)
    {
        return await _client.Get<List<FileMetadata>>($"api/{API_VERSION}/Files", null, pagination);
    }

    public async Task<ApiResponse<List<FileMetadata>>> ListFileMetadata(List<string> ids, PaginationFilter? pagination = null)
    {
        return await _client
            .Request<List<FileMetadata>>(HttpMethod.Get, $"api/{API_VERSION}/Files")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("ids", ids)
            .Execute();
    }

    public async Task<ApiResponse<FileMetadata>> GetFileMetadata(string id)
    {
        return await _client.Get<FileMetadata>($"api/{API_VERSION}/Files/{id}/metadata");
    }

    public async Task<ApiResponse<RegenerateFileOwnershipResponse>> RegenerateFileOwnershipToken(string id)
    {
        return await _client.Patch<RegenerateFileOwnershipResponse>($"api/{API_VERSION}/Files/{id}/RegenerateOwnershipToken");
    }

    public async Task<ApiResponse<ValidateFileOwnershipTokenResponse>> ValidateFileOwnershipToken(string fileId, ValidateFileOwnershipTokenRequest request)
    {
        return await _client.Post<ValidateFileOwnershipTokenResponse>($"api/{API_VERSION}/Files/{fileId}/ValidateOwnershipToken", request);
    }

    public async Task<ApiResponse<ClaimFileOwnershipResponse>> ClaimFileOwnership(string fileId, ClaimFileOwnershipRequest request)
    {
        return await _client.Patch<ClaimFileOwnershipResponse>($"api/{API_VERSION}/Files/{fileId}/ClaimOwnership", request);
    }

    public async Task<RawApiResponse> DownloadFile(string id)
    {
        return await _client
            .Request<RawApiResponse>(HttpMethod.Get, $"api/{API_VERSION}/Files/{id}")
            .Authenticate()
            .ExecuteRaw();
    }

    public async Task<ApiResponse<EmptyResponse>> DeleteFile(string id)
    {
        return await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Files/{id}");
    }
}
