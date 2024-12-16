using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Challenges.Jobs.Cleanup;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.AddHostedService<Worker>();

                services.AddApplication();

                services.AddSingleton<IUserContext, FakeUserContext>();

                services.AddDbContext<ChallengesDbContext>(dbContextOptions =>
                    dbContextOptions.UseSqlServer(configuration.GetSection("SqlDatabase")["ConnectionString"], sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ChallengesDbContext).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    }), ServiceLifetime.Singleton);

                services.AddSingleton<IChallengesRepository, ChallengesRepository>();
            });
    }
}

public class FakeUserContext : IUserContext
{
    public IdentityAddress GetAddress()
    {
        throw new NotSupportedException();
    }

    public IdentityAddress GetAddressOrNull()
    {
        throw new NotSupportedException();
    }

    public DeviceId GetDeviceId()
    {
        throw new NotSupportedException();
    }

    public DeviceId GetDeviceIdOrNull()
    {
        throw new NotSupportedException();
    }

    public string GetUserId()
    {
        throw new NotSupportedException();
    }

    public string GetUserIdOrNull()
    {
        throw new NotSupportedException();
    }

    public string GetUsername()
    {
        throw new NotSupportedException();
    }

    public string GetUsernameOrNull()
    {
        throw new NotSupportedException();
    }

    public string GetClientId()
    {
        throw new NotSupportedException();
    }

    public string? GetClientIdOrNull()
    {
        throw new NotSupportedException();
    }
}
