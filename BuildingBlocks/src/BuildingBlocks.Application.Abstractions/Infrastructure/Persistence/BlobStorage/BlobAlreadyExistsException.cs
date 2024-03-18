namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;

public class BlobAlreadyExistsException : Exception
{
    public BlobAlreadyExistsException(string blobName)
    {
        BlobName = blobName;
    }

    public string BlobName { get; }
}
