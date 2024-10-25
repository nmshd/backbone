﻿using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, Action<PersistenceOptions> setupOptions)
    {
        var options = new PersistenceOptions();
        setupOptions.Invoke(options);

        services.AddPersistence(options);
    }

    public static void AddPersistence(this IServiceCollection services, PersistenceOptions options)
    {
        services.AddDatabase(options.DbOptions);

        services.AddTransient<IAnnouncementsRepository, AnnouncementsRepository>();
    }
}

public class PersistenceOptions
{
    public Database.IServiceCollectionExtensions.DbOptions DbOptions { get; set; } = new();
}
