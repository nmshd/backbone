namespace Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;

public interface IBlobStorage
{
    void Add(string id, byte[] content);
    Task<byte[]> FindAsync(string id);
    Task<IAsyncEnumerable<string>> FindAllAsync(string? prefix = null);
    void Remove(string id);
    Task SaveAsync();
}