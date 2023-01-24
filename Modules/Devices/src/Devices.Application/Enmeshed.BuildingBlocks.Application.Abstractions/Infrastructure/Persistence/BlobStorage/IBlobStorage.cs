// ReSharper disable once CheckNamespace

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage
{
    public interface IBlobStorage
    {
        void Add(string id, byte[] content);
        Task<byte[]> FindAsync(string id);
        void Remove(string id);
        Task SaveAsync();
    }
}
