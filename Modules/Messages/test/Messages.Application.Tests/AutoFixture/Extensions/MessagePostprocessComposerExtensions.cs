using AutoFixture.Dsl;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Messages.Application.Tests.AutoFixture.Extensions;

public static class MessagePostprocessComposerExtensions
{
    public static IPostprocessComposer<Message> WithId(this IPostprocessComposer<Message> composer, MessageId id)
    {
        return composer.With(r => r.Id, id);
    }

    public static IPostprocessComposer<Message> WithSender(this IPostprocessComposer<Message> composer, IdentityAddress sender)
    {
        return composer.With(r => r.CreatedBy, sender);
    }
}
