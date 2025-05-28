namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;

public interface IBlobStorage
{
    void Add(string folder, string id, byte[] content);
    Task<byte[]> GetAsync(string folder, string id);
    Task<IAsyncEnumerable<string>> ListAsync(string folder, string? prefix = null);
    void Remove(string folder, string id);
    Task SaveAsync();
}
