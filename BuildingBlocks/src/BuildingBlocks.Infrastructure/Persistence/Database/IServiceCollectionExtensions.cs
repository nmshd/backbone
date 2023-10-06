﻿using Enmeshed.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSaveChangesTimeInterceptor(this IServiceCollection services)
    {
        if (EnvironmentVariables.DEBUG_PERFORMANCE)
            services.TryAddScoped<SaveChangesTimeInterceptor>();

        return services;
    }
}
