using AutoFixture.Dsl;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Domain.Entities;
using Messages.Domain.Ids;

namespace Messages.Application.Tests.AutoFixture.Extensions;

public static class RelationshipPostprocessComposerExtensions
{
    public static IPostprocessComposer<Relationship> WithId(this IPostprocessComposer<Relationship> composer, RelationshipId id)
    {
        return composer.With(r => r.Id, id);
    }

    public static IPostprocessComposer<Relationship> WithFrom(this IPostprocessComposer<Relationship> composer, IdentityAddress from)
    {
        return composer.With(r => r.From, from);
    }

    public static IPostprocessComposer<Relationship> WithTo(this IPostprocessComposer<Relationship> composer, IdentityAddress to)
    {
        return composer.With(r => r.To, to);
    }

    public static IPostprocessComposer<Relationship> NotTerminated(this IPostprocessComposer<Relationship> composer)
    {
        return composer.With(r => r.Status, RelationshipStatus.Active);
    }

    public static IPostprocessComposer<Relationship> Terminated(this IPostprocessComposer<Relationship> composer)
    {
        return composer.With(r => r.Status, RelationshipStatus.Terminated);
    }
}
