using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Tooling.Extensions;

public static class ServiceCollectionExtensions
{
    public static void Configure<TOptions>(this IServiceCollection services, TOptions options) where TOptions : class
    {
        services.AddOptions<TOptions>().Configure(options.CopyInto).ValidateOnStart();
    }

    public static void Configure<TOptions, TOptionsValidator>(this IServiceCollection services, TOptions options) where TOptions : class where TOptionsValidator : class, IValidateOptions<TOptions>
    {
        services.Configure(options);
        services.ConfigureOptions<TOptionsValidator>();
    }
}
