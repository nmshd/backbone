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
    public record CommandHandler(ILogger<CreateMessages> Logger, IHttpClientFactory HttpClientFactory) : IRequestHandler<Command, Unit>
    {
        private int _numberOfCreatedMessages;
        private long _totalMessages;
        private readonly Lock _lockObj = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(10);

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            _totalMessages = request.RelationshipAndMessages.Sum(relationship => relationship.NumberOfSentMessages);
            _numberOfCreatedMessages = 0;

            var tasks = request.Identities
                .Select(identity => ExecuteOuterCreateMessages(request, identity))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        private async Task ExecuteOuterCreateMessages(Command request, DomainIdentity senderIdentity)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                var recipientBag = GetRecipientIdentities(request, senderIdentity);
                stopwatch.Stop();

                Logger.LogDebug(" Semaphore.Count: {SemaphoreCount} - Recipient identities for Sender Identity {Address}/{ConfigurationAddress}/{Pool} found in {ElapsedMilliseconds} ms",
                    _semaphoreSlim.CurrentCount,
                    senderIdentity.IdentityAddress,
                    senderIdentity.ConfigurationIdentityAddress,
                    senderIdentity.PoolAlias,
                    stopwatch.ElapsedMilliseconds);

                foreach (var recipientIdentity in recipientBag.RecipientIdentities)
                {
                    await ExecuteInnerCreateMessages(request, recipientIdentity, senderIdentity, recipientBag);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task ExecuteInnerCreateMessages(Command request, DomainIdentity recipientIdentity, DomainIdentity senderIdentity, RecipientBag recipientBag)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            var sentMessages = await CreateMessages(request, recipientIdentity, senderIdentity, recipientBag);
            stopwatch.Stop();

            using (_lockObj.EnterScope())
            {
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

        private static async Task<List<MessageBag>> CreateMessages(
            Command request,
            DomainIdentity recipientIdentity,
            DomainIdentity senderIdentity,
            RecipientBag recipientBag)
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
                var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, senderIdentity.UserCredentials);

                var messageResponse = await sdkClient.Messages.SendMessage(new SendMessageRequest
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

                sentMessages.Add(new MessageBag(messageResponse.Result.Id, sdkClient.DeviceData!.DeviceId, recipientIdentity));
            }

            senderIdentity.SentMessages.AddRange(sentMessages);

            return sentMessages;
        }

        private static RecipientBag GetRecipientIdentities(Command request, DomainIdentity senderIdentity)
        {
            var recipientsRelationshipIds = request.RelationshipAndMessages
                .Where(relationship =>
                    senderIdentity.PoolAlias == relationship.SenderPoolAlias &&
                    senderIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress &&
                    relationship.NumberOfSentMessages > 0)
                .Select(relationship => new RelationshipIdBag(
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
                .Select(c => new RelationshipIdBag(c.ConfigurationIdentityAddress, c.PoolAlias))
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
