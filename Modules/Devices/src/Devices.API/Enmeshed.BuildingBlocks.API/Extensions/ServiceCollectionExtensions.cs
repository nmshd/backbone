using Autofac;
using Autofac.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static AutofacServiceProvider ToAutofacServiceProvider(this IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }
    }
}