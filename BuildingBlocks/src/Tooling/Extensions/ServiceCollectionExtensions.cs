using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Tooling.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void Configure<TOptions>(TOptions options) where TOptions : class
        {
            services.AddOptions<TOptions>().Configure(options.CopyInto).ValidateOnStart();
        }

        public void Configure<TOptions, TOptionsValidator>(TOptions options) where TOptions : class where TOptionsValidator : class, IValidateOptions<TOptions>
        {
            services.Configure(options);
            services.ConfigureOptions<TOptionsValidator>();
        }
    }
}
