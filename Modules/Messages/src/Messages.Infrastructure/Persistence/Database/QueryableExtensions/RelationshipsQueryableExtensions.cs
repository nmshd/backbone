using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;

public static class RelationshipsQueryableExtensions
{
    public static IQueryable<Relationship> WithParticipants(this IQueryable<Relationship> query, IdentityAddress participant1, IdentityAddress participant2)
    {
        return query.Where(r => r.From == participant1 && r.To == participant2 || r.From == participant2 && r.To == participant1);
    }

    public static IQueryable<Relationship> Active(this IQueryable<Relationship> query)
    {
        return query.Where(r => r.Status == RelationshipStatus.Active || r.Status == RelationshipStatus.Terminating);
    }
}
