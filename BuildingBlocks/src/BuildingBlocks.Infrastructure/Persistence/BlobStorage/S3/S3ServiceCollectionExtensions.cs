using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.S3;

public static class S3ServiceCollectionExtensions
{
    public static void AddS3(this IServiceCollection services,
        Action<S3BucketOptions> setupOptions)
    {
        var options = new S3BucketOptions();
        setupOptions.Invoke(options);

        services.AddS3(options);
    }

    public static void AddS3(this IServiceCollection services, S3BucketOptions options)
    {
        services.Configure<S3BucketOptions>(s3Options =>
        {
            s3Options.BucketName = options.BucketName;
            s3Options.AccessKeyId = options.AccessKeyId;
            s3Options.SecretAccessKey = options.SecretAccessKey;
            s3Options.ServiceUrl = options.ServiceUrl;
        });

        services.AddScoped<IBlobStorage, S3BlobStorage>();
    }
}

public class S3BucketOptions
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
