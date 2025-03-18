using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.S3;

public static class S3ServiceCollectionExtensions
{
    public static void AddS3(this IServiceCollection services,
        Action<S3BucketConfiguration> setupOptions)
    {
        var options = new S3BucketConfiguration();
        setupOptions.Invoke(options);

        services.AddS3(options);
    }

    public static void AddS3(this IServiceCollection services, S3BucketConfiguration configuration)
    {
        services.Configure<S3BucketConfiguration>(s3Options =>
        {
            s3Options.BucketName = configuration.BucketName;
            s3Options.AccessKeyId = configuration.AccessKeyId;
            s3Options.SecretAccessKey = configuration.SecretAccessKey;
            s3Options.ServiceUrl = configuration.ServiceUrl;
        });

        services.AddScoped<IBlobStorage, S3BlobStorage>();
    }
}

public class S3BucketConfiguration
{
    [Required]
    public string ServiceUrl { get; set; } = string.Empty;

    [Required]
    public string AccessKeyId { get; set; } = string.Empty;

    [Required]
    public string SecretAccessKey { get; set; } = string.Empty;

    [Required]
    public string BucketName { get; set; } = string.Empty;
}
