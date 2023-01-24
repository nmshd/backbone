using System.Reflection;
using Devices.Application.Infrastructure.Persistence;
using Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
            });
        });

        services.AddScoped<IDevicesDbContext, ApplicationDbContext>();
    }

    public class DbOptions
    {
        public string ConnectionString { get; set; }
        public RetryOptions RetryOptions { get; set; } = new();
    }

    public class RetryOptions
    {
        public byte MaxRetryCount { get; set; } = 15;
        public int MaxRetryDelayInSeconds { get; set; } = 30;
    }
}
