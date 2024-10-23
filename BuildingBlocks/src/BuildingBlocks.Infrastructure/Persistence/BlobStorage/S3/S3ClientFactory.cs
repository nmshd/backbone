using Amazon.S3;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.S3;

public class S3ClientFactory
{
    private readonly S3Options _options;

    public S3ClientFactory(IOptions<S3Options> options)
    {
        _options = options.Value;
    }

    public IAmazonS3 CreateClient()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = _options.ServiceUrl,
            ForcePathStyle = true
        };

        return new AmazonS3Client(_options.AccessKey, _options.SecretKey, config);
    }
}
