using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.Ionos;
public static class IonosS3ServiceCollectionExtensions
{
    public static void AddIonosS3(this IServiceCollection services,
        Action<IonosS3Options> setupOptions)
    {
        var options = new IonosS3Options();
        setupOptions.Invoke(options);

        services.AddIonosS3(options);
    }

    public static void AddIonosS3(this IServiceCollection services, IonosS3Options options)
    {
        services.Configure<IonosS3Options>(opt =>
        {
            opt.ServiceUrl = options.ServiceUrl;
            opt.AccessKey = options.AccessKey;
            opt.SecretKey = options.SecretKey;
            opt.BucketName = options.BucketName;
        });

        services.AddSingleton<IonosS3ClientFactory>();
        services.AddScoped<IBlobStorage, IonosS3BlobStorage>();
    }
}

public class IonosS3Options
{
    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? BucketName { get; set; }
}
