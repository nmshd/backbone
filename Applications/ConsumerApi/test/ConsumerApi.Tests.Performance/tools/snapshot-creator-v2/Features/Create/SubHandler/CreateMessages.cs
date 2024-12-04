using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Crypto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateMessages
{
    public record Command(
        List<DomainIdentity> Identities,
        List<RelationshipAndMessages> RelationshipAndMessages,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CreateMessages> Logger) : IRequestHandler<Command, Unit>
    {
        private int _numberOfCreatedMessages;
        private long _totalMessages;
        private readonly Lock _lockObj = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(Environment.ProcessorCount);
        private readonly SemaphoreSlim _createMessagesSemaphore = new(Environment.ProcessorCount);

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            _totalMessages = request.RelationshipAndMessages.Sum(relationship => relationship.NumberOfSentMessages);
            _numberOfCreatedMessages = 0;

            var senderIdentities = request.Identities
                .Where(identity => request.RelationshipAndMessages.Any(relationship =>
                    identity.PoolAlias == relationship.SenderPoolAlias &&
                    identity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress &&
                    relationship.NumberOfSentMessages > 0))
                .ToArray();

            var sum = senderIdentities.Sum(s => s.NumberOfSentMessages);
            if (sum != _totalMessages)
            {
                throw new InvalidOperationException($"Mismatch between configured relationships and connector identities. SenderIdentities.SumMessages: {sum}, TotalMessages: {_totalMessages}");
            }

            var tasks = senderIdentities.Select(senderIdentity => ExecuteOuterCreateMessages(request, senderIdentity)).ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        private async Task ExecuteOuterCreateMessages(Command request, DomainIdentity senderIdentity)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var recipientBag = GetRecipientIdentities(request, senderIdentity);

                var tasks = recipientBag.RecipientIdentities
                    .Select(recipientIdentity => ExecuteInnerCreateMessages(request, recipientIdentity, senderIdentity, recipientBag))
                    .ToArray();

                await Task.WhenAll(tasks);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task ExecuteInnerCreateMessages(Command request, DomainIdentity recipientIdentity, DomainIdentity senderIdentity, RecipientBag recipientBag)
        {
            await _createMessagesSemaphore.WaitAsync();
            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                var skdClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, senderIdentity.UserCredentials);
                var sentMessages = await CreateMessages(recipientIdentity, senderIdentity, recipientBag, skdClient);
                stopwatch.Stop();

                using (_lockObj.EnterScope())
                {
                    senderIdentity.SentMessages.AddRange(sentMessages);
                    _numberOfCreatedMessages += sentMessages.Count;
                }

                Logger.LogDebug(
                    "Created {CreatedMessages}/{TotalMessages} messages. Messages from Sender Identity {SenderAddress}/{SenderConfigurationAddress}/{SenderPool} to Recipient Identity {RecipientAddress}/{RecipientConfigurationAddress}/{RecipientPool} created in {ElapsedMilliseconds} ms",
                    _numberOfCreatedMessages,
                    _totalMessages,
                    senderIdentity.IdentityAddress,
                    senderIdentity.ConfigurationIdentityAddress,
                    senderIdentity.PoolAlias,
                    recipientIdentity.IdentityAddress,
                    recipientIdentity.ConfigurationIdentityAddress,
                    recipientIdentity.PoolAlias,
                    stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                _createMessagesSemaphore.Release();
            }
        }

        private static async Task<List<MessageBag>> CreateMessages(DomainIdentity recipientIdentity,
            DomainIdentity senderIdentity,
            RecipientBag recipientBag,
            Client senderIdentitySdkClient)
        {
            if (recipientIdentity.IdentityAddress is null)
            {
                throw new InvalidOperationException(BuildErrorDetails("Recipient identity address is null.", senderIdentity, recipientIdentity));
            }

            List<MessageBag> sentMessages = [];

            var numSentMessages = recipientBag.RelationshipIds.First(relationshipIdBag =>
                relationshipIdBag.IdentityAddress == recipientIdentity.ConfigurationIdentityAddress &&
                relationshipIdBag.PoolAlias == recipientIdentity.PoolAlias).NumberOfSentMessages;

            for (var i = 0; i < numSentMessages; i++)
            {
                var messageResponse = await senderIdentitySdkClient.Messages.SendMessage(new SendMessageRequest
                {
                    Recipients =
                    [
                        new SendMessageRequestRecipientInformation
                        {
                            Address = recipientIdentity.IdentityAddress,
                            EncryptedKey = ConvertibleString.FromUtf8(new string('A', 152)).BytesRepresentation
                        }
                    ],
                    Attachments = [],
                    Body = ConvertibleString.FromUtf8("Message body").BytesRepresentation
                });


                if (messageResponse.IsError)
                {
                    throw new InvalidOperationException(BuildErrorDetails(
                        "Failed to send message.",
                        senderIdentity,
                        recipientIdentity,
                        messageResponse));
                }

                if (messageResponse.Result is null) continue;

                sentMessages.Add(new MessageBag(messageResponse.Result.Id, senderIdentitySdkClient.DeviceData!.DeviceId, recipientIdentity));
            }

            return sentMessages;
        }

        private static RecipientBag GetRecipientIdentities(Command request, DomainIdentity senderIdentity)
        {
            var recipientsRelationshipIds = request.RelationshipAndMessages
                .Where(relationship =>
                    senderIdentity.PoolAlias == relationship.SenderPoolAlias &&
                    senderIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress &&
                    relationship.NumberOfSentMessages > 0)
                .Select(relationship => new RelationshipIdentityBag(
                    relationship.RecipientIdentityAddress,
                    relationship.RecipientPoolAlias,
                    relationship.NumberOfSentMessages))
                .ToArray();

            var recipientIdentities = request.Identities
                .Where(recipient => recipientsRelationshipIds.Any(relationship =>
                    recipient.PoolAlias == relationship.PoolAlias &&
                    recipient.ConfigurationIdentityAddress == relationship.IdentityAddress))
                .ToArray();

            var recipientRelationshipIdsWithoutNumMessages = recipientsRelationshipIds
                .Select(relationshipIdBag => relationshipIdBag with { NumberOfSentMessages = default })
                .OrderBy(relationshipIdBag => relationshipIdBag.PoolAlias)
                .ThenBy(relationshipIdBag => relationshipIdBag.IdentityAddress)
                .ToArray();

            var recipientIdentityIds = recipientIdentities
                .Select(c => new RelationshipIdentityBag(c.ConfigurationIdentityAddress, c.PoolAlias))
                .OrderBy(relationshipIdBag => relationshipIdBag.PoolAlias)
                .ThenBy(relationshipIdBag => relationshipIdBag.IdentityAddress)
                .ToArray();

            return !recipientRelationshipIdsWithoutNumMessages.SequenceEqual(recipientIdentityIds)
                ? throw new InvalidOperationException(BuildRelationshipErrorDetails("Mismatch between configured relationships and connector identities.",
                    senderIdentity,
                    recipientRelationshipIdsWithoutNumMessages,
                    recipientIdentityIds))
                : new RecipientBag(recipientsRelationshipIds, recipientIdentities);
        }
    }
}
