using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Files;

public class FilesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<CreateFileResponse>> UploadFile(CreateFileRequest request) => await _client
        .Request<CreateFileResponse>(HttpMethod.Post, "Files")
        .Authenticate()
        .WithMultipartForm(request.ToMultipartContent())
        .Execute();

    public async Task<ConsumerApiResponse<List<FileMetadata>>> ListFileMetadata(PaginationFilter? pagination = null)
        => await _client.Get<List<FileMetadata>>("Files", null, pagination);

    public async Task<ConsumerApiResponse<List<FileMetadata>>> ListFileMetadata(List<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<List<FileMetadata>>(HttpMethod.Get, "Files")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ConsumerApiResponse<FileMetadata>> GetFileMetadata(string id) => await _client.Get<FileMetadata>($"Files/{id}/metadata");

    public async Task<RawConsumerApiResponse> DownloadFile(string id) => await _client
        .Request<RawConsumerApiResponse>(HttpMethod.Get, $"Files/{id}")
        .Authenticate()
        .ExecuteRaw();
}
