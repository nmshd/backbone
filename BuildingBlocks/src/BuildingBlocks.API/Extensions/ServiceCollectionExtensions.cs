using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.BuildingBlocks.API.Extensions;

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