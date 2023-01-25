using AutoFixture;
using AutoFixture.Dsl;
using Messages.Application.Messages.Commands.SendMessage;

namespace Messages.Application.Tests.AutoFixture.Extensions;

public static class SendMessageCommandPostprocessComposerExtensions
{
    public static IPostprocessComposer<SendMessageCommand> WithDoNotSendBefore(this IPostprocessComposer<SendMessageCommand> composer, DateTime? date)
    {
        return composer.With(c => c.DoNotSendBefore, date);
    }

    public static IPostprocessComposer<SendMessageCommand> WithBody(this IPostprocessComposer<SendMessageCommand> composer, byte[] body)
    {
        return composer.With(c => c.Body, body);
    }

    public static IPostprocessComposer<SendMessageCommand> WithoutBody(this IPostprocessComposer<SendMessageCommand> composer)
    {
        return composer.With(c => c.Body, default(byte[]));
    }

    public static IPostprocessComposer<SendMessageCommand> WithBodySize(this IPostprocessComposer<SendMessageCommand> composer, int size)
    {
        return composer.With(c => c.Body, new byte[size]);
    }

    public static IPostprocessComposer<SendMessageCommand> WithRecipients(this IPostprocessComposer<SendMessageCommand> composer, params SendMessageCommandRecipientInformation[] recipients)
    {
        return composer.With(c => c.Recipients, new List<SendMessageCommandRecipientInformation>(recipients));
    }

    public static IPostprocessComposer<SendMessageCommand> WithoutRecipients(this IPostprocessComposer<SendMessageCommand> composer)
    {
        return composer.With(c => c.Recipients, default(List<SendMessageCommandRecipientInformation>));
    }

    public static IPostprocessComposer<SendMessageCommand> WithAttachments(this IPostprocessComposer<SendMessageCommand> composer, params SendMessageCommandAttachment[] attachments)
    {
        return composer.With(c => c.Attachments, new List<SendMessageCommandAttachment>(attachments));
    }

    public static IPostprocessComposer<SendMessageCommand> WithNAttachments(this IPostprocessComposer<SendMessageCommand> composer, int n)
    {
        return composer.With(c => c.Attachments, composer.CreateMany<SendMessageCommandAttachment>(n).ToArray());
    }
}
