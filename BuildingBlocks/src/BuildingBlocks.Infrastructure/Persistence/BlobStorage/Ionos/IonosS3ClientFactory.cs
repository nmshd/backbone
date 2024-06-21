using Amazon.S3;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.Ionos;
public class IonosS3ClientFactory
{
    private readonly IonosS3Options _options;

    public IonosS3ClientFactory(IOptions<IonosS3Options> options)
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
