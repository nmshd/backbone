using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class PnsRegistrationQueryableExtensions
{
    public static async Task<List<PnsRegistration>> FindWithoutAppId(this IQueryable<PnsRegistration> query, CancellationToken cancellationToken)
    {
        return await query.Where(x => x.AppId == null || x.AppId == string.Empty).ToListAsync(cancellationToken);
    }
}
