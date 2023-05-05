using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.BuildingBlocks.API;
public interface IStartup
{
    void ConfigureServices(IServiceCollection services, IConfigurationSection configuration);
    void Configure(WebApplication app);
    void ConfigureEventBus(IEventBus eventBus);
}
