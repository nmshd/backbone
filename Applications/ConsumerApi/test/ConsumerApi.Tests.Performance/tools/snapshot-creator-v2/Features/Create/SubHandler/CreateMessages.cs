using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Crypto;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record CreateMessages
{
    public record Command(
        List<DomainIdentity> Identities,
        List<RelationshipAndMessages> RelationshipAndMessages,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            foreach (var senderIdentity in request.Identities)
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
                    .ToList();

                var recipientIdentities = request.Identities
                    .Where(recipient => recipientsRelationshipIds.Any(relationship =>
                        recipient.PoolAlias == relationship.PoolAlias &&
                        recipient.ConfigurationIdentityAddress == relationship.IdentityAddress))
                    .ToList();

                var recipientRelationshipIdsWithoutNumMessages = recipientsRelationshipIds
                    .Select(relationshipIdBag => relationshipIdBag with { NumberOfSentMessages = default })
                    .OrderBy(relationshipIdBag => relationshipIdBag.PoolAlias)
                    .ThenBy(relationshipIdBag => relationshipIdBag.IdentityAddress)
                    .ToList();

                var recipientIdentityIds = recipientIdentities
                    .Select(c => new RelationshipIdBag(c.ConfigurationIdentityAddress, c.PoolAlias))
                    .OrderBy(relationshipIdBag => relationshipIdBag.PoolAlias)
                    .ThenBy(relationshipIdBag => relationshipIdBag.IdentityAddress)
                    .ToList();

                if (!recipientRelationshipIdsWithoutNumMessages.SequenceEqual(recipientIdentityIds))
                {
                    throw new InvalidOperationException(BuildRelationshipErrorDetails("Mismatch between configured relationships and connector identities.",
                        senderIdentity,
                        recipientRelationshipIdsWithoutNumMessages,
                        recipientIdentityIds));
                }

                foreach (var recipientIdentity in recipientIdentities)
                {
                    if (recipientIdentity.IdentityAddress is null)
                    {
                        throw new InvalidOperationException(BuildErrorDetails("Recipient identity address is null.", senderIdentity, recipientIdentity));
                    }

                    var numSentMessages = recipientsRelationshipIds.First(relationshipIdBag =>
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

                        senderIdentity.SentMessages.Add(new MessageBag(messageResponse.Result.Id, sdkClient.DeviceData!.DeviceId, recipientIdentity));
                    }
                }
            }

            return Unit.Value;
        }
    }
}
