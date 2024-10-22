using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record Identity(int Id, IdentityPoolType IdentityPoolType, IdentityPoolConfiguration IdentityPoolConfiguration)
{
    public List<RelationshipAndMessages> RelationshipAndMessages { get; } = [];
    public bool HasAvailableRelationships => NumberOfRelationships > 0;

    public string PoolAlias => IdentityPoolConfiguration.Alias;

    public long NumberOfRelationships { get; private set; } = IdentityPoolConfiguration.NumberOfRelationships;

    public long DecrementAvailableRelationships() =>
        NumberOfRelationships == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_RELATIONSHIPS_AVAILABLE) : NumberOfRelationships--;

    public long AvailableMessagesToSend { get; private set; } = IdentityPoolConfiguration.NumberOfSentMessages;

    public long DecrementActualNumberOfMessagesToSend() =>
        AvailableMessagesToSend == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_MESSAGES_TO_SEND) : AvailableMessagesToSend--;

    private bool HasMessagesToSend => AvailableMessagesToSend > 0;


    public long ActualNumberOfReceivedMessages { get; private set; }


    public long IncrementActualNumberOfReceivedMessages() =>
        CanReceiveMessages ? ActualNumberOfReceivedMessages++ : throw new InvalidOperationException(IDENTITY_NO_MORE_MESSAGES_TO_RECEIVE);

    /// <summary>
    /// Number of messages to send per relationship
    /// </summary>
    private long NumberOfMessagesToSendPerRelationship => IdentityPoolConfiguration.NumberOfSentMessages / IdentityPoolConfiguration.NumberOfRelationships;

    /// <summary>
    /// Modulo (remaining part representing the fractional part of <see cref="NumberOfMessagesToSendPerRelationship"/> division)
    /// Number of messages to send per relationship
    /// </summary>
    private long ModuloMessagesToSendPerRelationship => IdentityPoolConfiguration.NumberOfSentMessages % IdentityPoolConfiguration.NumberOfRelationships;

    /// <summary>
    /// Determines if the Identity can receive messages
    /// </summary>
    private bool CanReceiveMessages => ActualNumberOfReceivedMessages < IdentityPoolConfiguration.NumberOfReceivedMessages;

    /// <summary>
    /// Number of messages to receive per relationship
    /// </summary>
    private long NumberOfMessagesReceivedPerRelationship => IdentityPoolConfiguration.NumberOfReceivedMessages / IdentityPoolConfiguration.NumberOfRelationships;

    /// <summary>
    /// Modulo (remaining part representing the fractional part of <see cref="NumberOfMessagesReceivedPerRelationship"/> division)
    /// Number of messages to receive per relationship
    /// </summary>
    private long ModuloReceivedMessagesPerRelationship => IdentityPoolConfiguration.NumberOfReceivedMessages % IdentityPoolConfiguration.NumberOfRelationships;

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
                var messagesToSend = GetMessagesToSend(relationship);

                if (messagesToSend == 0)
                {
                    return;
                }

                var messagesToReceive = GetMessagesToReceive(relationship);

                ApplyReceiverMessages(messagesToReceive, relationship, messagesToSend);

                return;
            }
        }

        // Add messages to send to the receiver
        foreach (var relationship in RelationshipAndMessages)
        {
            var messagesToSend = GetMessagesToSend(relationship);

            if (messagesToSend == 0)
            {
                // No more messages to send
                return;
            }

            var messagesToReceive = GetMessagesToReceive(relationship);

            ApplyReceiverMessages(messagesToReceive, relationship, messagesToSend);
        }

        return;


        long GetMessagesToReceive(RelationshipAndMessages relationshipAndMessages)
        {
            long messagesToReceive;

            if (NumberOfMessagesReceivedPerRelationship > relationshipAndMessages.NumberOfSentMessages)
            {
                messagesToReceive = NumberOfMessagesReceivedPerRelationship - relationshipAndMessages.NumberOfSentMessages;
            }
            else if (!relationshipAndMessages.ModuloReceivedApplied)
            {
                relationshipAndMessages.ModuloReceivedApplied = true;
                messagesToReceive = ModuloReceivedMessagesPerRelationship;
            }
            else
            {
                messagesToReceive = 0;
            }

            return messagesToReceive;
        }

        long GetMessagesToSend(RelationshipAndMessages relationship)
        {
            if (!HasMessagesToSend)
            {
                return 0;
            }

            long messagesToSend;

            if (AvailableMessagesToSend >= NumberOfMessagesToSendPerRelationship)
            {
                messagesToSend = NumberOfMessagesToSendPerRelationship;
            }
            else if (!relationship.ModuloSentApplied)
            {
                relationship.ModuloSentApplied = true;
                messagesToSend = ModuloMessagesToSendPerRelationship;
            }
            else
            {
                messagesToSend = 0;
            }

            return messagesToSend;
        }

        void ApplyReceiverMessages(long messagesToReceive, RelationshipAndMessages relationship, long messagesToSend)
        {
            for (var i = 0; i < messagesToReceive; i++)
            {
                if (!receiverIdentity.CanReceiveMessages)
                {
                    break;
                }

                relationship.NumberOfSentMessages++;

                DecrementActualNumberOfMessagesToSend();
                IncrementActualNumberOfReceivedMessages();

                messagesToSend--;

                if (messagesToSend == 0)
                {
                    // No more messages to send
                    break;
                }
            }
        }
    }
}
