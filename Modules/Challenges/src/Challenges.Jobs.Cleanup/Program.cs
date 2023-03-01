using System.Reflection;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
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

                services.AddSingleton<IChallengesDbContext, ChallengesDbContext>();
            });
    }
}

public class FakeUserContext : IUserContext
{
    public IdentityAddress GetAddress()
    {
        throw new NotImplementedException();
    }

    public IdentityAddress GetAddressOrNull()
    {
        throw new NotImplementedException();
    }

    public DeviceId GetDeviceId()
    {
        throw new NotImplementedException();
    }

    public DeviceId GetDeviceIdOrNull()
    {
        throw new NotImplementedException();
    }

    public string GetUserId()
    {
        throw new NotImplementedException();
    }

    public string GetUserIdOrNull()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetRoles()
    {
        throw new NotImplementedException();
    }

    public SubscriptionPlan GetSubscriptionPlan()
    {
        throw new NotImplementedException();
    }
}
