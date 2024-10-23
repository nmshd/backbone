using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record Identity(int Id, IdentityPoolType IdentityPoolType, IdentityPoolConfiguration IdentityPoolConfiguration)
{
    public List<RelationshipAndMessages> RelationshipAndMessages { get; } = [];

    public bool HasAvailableRelationships => NumberOfRelationships > 0;

    public string PoolAlias => IdentityPoolConfiguration.Alias;

    public int NumberOfRelationships { get; private set; } = IdentityPoolConfiguration.NumberOfRelationships;

    public int DecrementAvailableRelationships() => NumberOfRelationships == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_RELATIONSHIPS_AVAILABLE) : NumberOfRelationships--;

    public int SendableMessages { get; private set; } = IdentityPoolConfiguration.NumberOfSentMessages;

    public int DecrementSendableMessages(int count) => SendableMessages == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_MESSAGES_TO_SEND) : SendableMessages -= count;

    private bool HasMessagesToSend => SendableMessages > 0;

    private bool CanReceiveMessages => ReceiveableMessages > 0;

    public int ReceiveableMessages { get; private set; } = IdentityPoolConfiguration.NumberOfReceivedMessages;

    public int DecrementReceivableMessages(int count) => ReceiveableMessages == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_MESSAGES_TO_RECEIVE) : ReceiveableMessages -= count;

    private int MessagesToSendPerRelationship => IdentityPoolConfiguration.NumberOfSentMessages / IdentityPoolConfiguration.NumberOfRelationships;

    private int MessagesToReceivePerRelationship => IdentityPoolConfiguration.NumberOfReceivedMessages / IdentityPoolConfiguration.NumberOfRelationships;


    public void ConfigureMessagesSentTo(Identity receiverIdentity)
    {
        // Sender Identity has no messages to send
        if (!HasMessagesToSend) return;

        if (receiverIdentity.CanReceiveMessages)
        {
            // Relationship between sender and receiver
            var relationship = RelationshipAndMessages.FirstOrDefault(rm => rm.ReceiverIdentityId == receiverIdentity.Id);

            if (relationship != null)
            {
                if (!HasMessagesToSend)
                {
                    return;
                }

                var messagesToSend = GetMessagesToSend();
                var messagesToReceive = GetMessagesToReceive(receiverIdentity);

                ApplyReceiverMessages(messagesToSend, messagesToReceive, relationship);

                return;
            }
        }

        // Add messages to any other available receiver already has a relationship to that sender
        foreach (var relationship in RelationshipAndMessages)
        {
            if (!HasMessagesToSend)
            {
                // No more messages to send
                return;
            }

            var messagesToSend = GetMessagesToSend();
            var messagesToReceive = GetMessagesToReceive(relationship.ReceiverIdentity);

            ApplyReceiverMessages(messagesToSend, messagesToReceive, relationship);
        }

        return;


        int GetMessagesToSend() => MessagesToSendPerRelationship < SendableMessages ? MessagesToSendPerRelationship : SendableMessages;

        int GetMessagesToReceive(Identity receiver) => receiver.MessagesToReceivePerRelationship < receiver.ReceiveableMessages
            ? receiver.MessagesToReceivePerRelationship
            : receiver.ReceiveableMessages;

        void ApplyReceiverMessages(int senderMessages, int receiverMessages, RelationshipAndMessages identityRelationship)
        {
            var messages = senderMessages <= receiverMessages ? senderMessages : receiverMessages;


            identityRelationship.NumberOfSentMessages += messages;
            identityRelationship.NumberOfReceivedMessages += messages;

            DecrementSendableMessages(messages);
            DecrementReceivableMessages(messages);
        }
    }
}
