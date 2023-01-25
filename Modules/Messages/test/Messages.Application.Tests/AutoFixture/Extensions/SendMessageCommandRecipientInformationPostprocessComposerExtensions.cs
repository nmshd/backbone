using AutoFixture;
using AutoFixture.Dsl;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Application.Messages.Commands.SendMessage;

namespace Messages.Application.Tests.AutoFixture.Extensions;

public static class SendMessageCommandRecipientInformationPostprocessComposerExtensions
{
    public static IPostprocessComposer<SendMessageCommandRecipientInformation> WithId(this IPostprocessComposer<SendMessageCommandRecipientInformation> composer, IdentityAddress id)
    {
        return composer.With(c => c.Address, id);
    }

    public static IPostprocessComposer<SendMessageCommandRecipientInformation> WithEncryptedKey(this IPostprocessComposer<SendMessageCommandRecipientInformation> composer, byte[] key)
    {
        return composer.With(c => c.EncryptedKey, key);
    }

    public static IPostprocessComposer<SendMessageCommandRecipientInformation> WithEncryptedKeyLength(this IPostprocessComposer<SendMessageCommandRecipientInformation> composer, int length)
    {
        return composer.With(c => c.EncryptedKey, composer.CreateMany<byte>(length).ToArray());
    }
}
