using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.S3;

public static class S3ServiceCollectionExtensions
{
    public static void AddS3(this IServiceCollection services,
        Action<S3Options> setupOptions)
    {
        var options = new S3Options();
        setupOptions.Invoke(options);

        services.AddS3(options);
    }

    public static void AddS3(this IServiceCollection services, S3Options options)
    {
        services.Configure<S3Options>(opt =>
        {
            opt.ServiceUrl = options.ServiceUrl;
            opt.AccessKey = options.AccessKey;
            opt.SecretKey = options.SecretKey;
            opt.BucketName = options.BucketName;
        });

        services.AddScoped<IBlobStorage, S3BlobStorage>();
    }
}

public class S3Options
{
    public string ServiceUrl { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
}
