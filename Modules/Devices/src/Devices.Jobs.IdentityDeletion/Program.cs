﻿using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Jobs.IdentityDeletion.Extensions;
using Backbone.Modules.Devices.Application.AutoMapper;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                configuration.Sources.Clear();
                var env = hostContext.HostingEnvironment;

                configuration
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    configuration.AddUserSecrets(appAssembly, optional: true);
                }

                configuration.AddEnvironmentVariables();
                configuration.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.AddHostedService<Worker>();

                var eventBusConfiguration = configuration.GetSection("Infrastructure").Get<InfrastructureConfiguration>().EventBus;
                var databaseConfiguration = configuration.GetSection("Infrastructure").Get<InfrastructureConfiguration>().SqlDatabase;

                services.AddEventBus(eventBusConfiguration);
                services.AddDatabase(options =>
                {
                    options.Provider = databaseConfiguration.Provider;
                    options.ConnectionString = databaseConfiguration.ConnectionString;
                    options.RetryOptions = databaseConfiguration.RetryOptions;
                });
                services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

                services.AddMediatR(o =>
                {
                    o.RegisterServicesFromAssemblyContaining<UpdateDeletionProcessesCommand>()
                    .AddOpenBehavior(typeof(LoggingBehavior<,>));
                });
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
