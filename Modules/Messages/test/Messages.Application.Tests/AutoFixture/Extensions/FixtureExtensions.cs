using AutoFixture;
using AutoFixture.Dsl;
using Messages.Application.Messages.Commands.SendMessage;
using Messages.Domain.Entities;

namespace Messages.Application.Tests.AutoFixture.Extensions;

public static class FixtureExtensions
{
    public static ICustomizationComposer<T> BuildWithDefaultCustomizations<T>(this Fixture fixture)
    {
        if (typeof(T) == typeof(Relationship))
            return (ICustomizationComposer<T>) fixture
                .Build<Relationship>()
                .NotTerminated();

        if (typeof(T) == typeof(SendMessageCommand))
            return (ICustomizationComposer<T>) fixture
                .Build<SendMessageCommand>()
                .WithDoNotSendBefore(null)
                .WithBody(new byte[] {0, 1, 2})
                .WithRecipients(fixture.BuildWithDefaultCustomizations<SendMessageCommandRecipientInformation>().Create())
                .WithAttachments(fixture.BuildWithDefaultCustomizations<SendMessageCommandAttachment>().Create());

        if (typeof(T) == typeof(SendMessageCommandRecipientInformation))
            return (ICustomizationComposer<T>) fixture
                .Build<SendMessageCommandRecipientInformation>()
                .WithEncryptedKey(fixture.CreateMany<byte>(200).ToArray());

        return fixture.Build<T>();
    }

    public static T CreateWithDefaultCustomizations<T>(this Fixture fixture)
    {
        return fixture.BuildWithDefaultCustomizations<T>().Create();
    }
}
