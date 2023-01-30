using AutoFixture.Dsl;
using Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;
using Backbone.Modules.Messages.Domain.Ids;

namespace Messages.Application.Tests.AutoFixture.Extensions;

public static class SendMessageCommandAttachmentPostprocessComposerExtensions
{
    public static IPostprocessComposer<SendMessageCommandAttachment> WithId(this IPostprocessComposer<SendMessageCommandAttachment> composer, FileId id)
    {
        return composer.With(c => c.Id, id);
    }
}
