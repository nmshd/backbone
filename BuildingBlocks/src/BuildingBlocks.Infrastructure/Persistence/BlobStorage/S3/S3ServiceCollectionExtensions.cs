using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.S3;

public static class S3ServiceCollectionExtensions
{
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
    public required string ServiceUrl { get; set; }

    [Required]
    public required string AccessKeyId { get; set; }

    [Required]
    public required string SecretAccessKey { get; set; }

    [Required]
    public required string BucketName { get; set; }
}
