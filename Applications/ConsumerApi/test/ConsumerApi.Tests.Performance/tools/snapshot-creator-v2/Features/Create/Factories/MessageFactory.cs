using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public class MessageFactory(ILogger<MessageFactory> logger, IConsumerApiHelper consumerApiHelper) : IMessageFactory
{
    public long TotalConfiguredMessages { get; set; }
    public int TotalCreatedMessages { get; private set; }

    private readonly Lock _lockObj = new();
    private readonly SemaphoreSlim _createSemaphore = new(MaxDegreeOfParallelism);
    private readonly SemaphoreSlim _createMessagesSemaphore = new(MaxDegreeOfParallelism);

    internal int GetCreateSemaphoreCurrentCount() => _createSemaphore.CurrentCount;
    internal int GetCreateMessagesSemaphoreCurrentCount() => _createMessagesSemaphore.CurrentCount;

    internal static int MaxDegreeOfParallelism => Environment.ProcessorCount;

    public async Task Create(CreateMessages.Command request, DomainIdentity senderIdentity)
    {
        await _createSemaphore.WaitAsync();

        try
        {
            var recipientBag = GetRecipientIdentities(request, senderIdentity);

            var tasks = recipientBag.RecipientDomainIdentities
                .Select(recipientIdentity => CreateMessages(request, recipientIdentity, senderIdentity, recipientBag))
                .ToArray();

            await Task.WhenAll(tasks);
        }
        finally
        {
            _createSemaphore.Release();
        }
    }

    private async Task CreateMessages(CreateMessages.Command request, DomainIdentity recipientIdentity, DomainIdentity senderIdentity, RecipientBag recipientBag)
    {
        await _createMessagesSemaphore.WaitAsync();
        try
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            var skdClient = consumerApiHelper.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, senderIdentity.UserCredentials);

            var sentMessages = await InnerCreateMessages(recipientIdentity, senderIdentity, recipientBag, skdClient);
            stopwatch.Stop();

            using (_lockObj.EnterScope())
            {
                senderIdentity.SentMessages.AddRange(sentMessages);
                TotalCreatedMessages += sentMessages.Count;
            }

            logger.LogDebug(
                "Created {CreatedMessages}/{TotalMessages} messages. Messages from Sender Identity {SenderAddress}/{SenderConfigurationAddress}/{SenderPool} to Recipient Identity {RecipientAddress}/{RecipientConfigurationAddress}/{RecipientPool} created in {ElapsedMilliseconds} ms",
                TotalCreatedMessages,
                TotalConfiguredMessages,
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

    private async Task<List<MessageBag>> InnerCreateMessages(DomainIdentity recipientIdentity,
        DomainIdentity senderIdentity,
        RecipientBag recipientBag,
        Client senderIdentitySdkClient)
    {
        if (recipientIdentity.IdentityAddress is null)
        {
            throw new InvalidOperationException(BuildErrorDetails("Recipient identity address is null.", senderIdentity, recipientIdentity));
        }

        List<MessageBag> sentMessages = [];

        var numSentMessages = recipientBag.RelationshipIdentityBags.First(relationshipIdBag =>
            relationshipIdBag.IdentityAddress == recipientIdentity.ConfigurationIdentityAddress &&
            relationshipIdBag.PoolAlias == recipientIdentity.PoolAlias).NumberOfSentMessages;

        for (var i = 0; i < numSentMessages; i++)
        {
            var messageResponse = await consumerApiHelper.SendMessage(recipientIdentity, senderIdentitySdkClient);

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

    private static RecipientBag GetRecipientIdentities(CreateMessages.Command request, DomainIdentity senderIdentity)
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

        VerifyRecipientConfiguration(senderIdentity, recipientsRelationshipIds, recipientIdentities);

        return new RecipientBag(recipientsRelationshipIds, recipientIdentities);
    }

    private static void VerifyRecipientConfiguration(DomainIdentity senderIdentity,
        RelationshipIdentityBag[] recipientsRelationshipIds, DomainIdentity[] recipientIdentities)
    {
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

        var isValid = recipientRelationshipIdsWithoutNumMessages.SequenceEqual(recipientIdentityIds);

        if (isValid) return;

        throw new InvalidOperationException(BuildRelationshipErrorDetails("Mismatch between configured relationships and connector identities.",
            senderIdentity,
            recipientRelationshipIdsWithoutNumMessages,
            recipientIdentityIds));
    }
}
