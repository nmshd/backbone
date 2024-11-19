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
                    .Select(relationship =>
                    (
                        relationship.RecipientIdentityAddress,
                        relationship.RecipientPoolAlias,
                        relationship.NumberOfSentMessages
                    ))
                    .ToList();


                var recipientIdentities = request.Identities
                    .Where(recipient => recipientsRelationshipIds.Any(relationship =>
                        recipient.PoolAlias == relationship.RecipientPoolAlias &&
                        recipient.ConfigurationIdentityAddress == relationship.RecipientIdentityAddress))
                    .ToList();

                var recipientRelationshipIdsWithoutNumMessages = recipientIdentities
                    .Select(c =>
                    (
                        c.ConfigurationIdentityAddress,
                        c.PoolAlias
                    ))
                    .OrderBy(c => c.PoolAlias)
                    .ThenBy(c => c.ConfigurationIdentityAddress)
                    .ToList();

                var recipientIdentityIds = recipientIdentities
                    .Select(c =>
                    (
                        c.ConfigurationIdentityAddress,
                        c.PoolAlias
                    ))
                    .OrderBy(c => c.PoolAlias)
                    .ThenBy(c => c.ConfigurationIdentityAddress)
                    .ToList();

                if (recipientsRelationshipIds.Count != recipientIdentityIds.Count ||
                    !recipientRelationshipIdsWithoutNumMessages.SequenceEqual(recipientIdentityIds))
                {
                    throw new InvalidOperationException("Mismatch between configured relationships and connector identities." +
                                                        Environment.NewLine +
                                                        $"sender-identity: {senderIdentity.IdentityAddress}/{senderIdentity.ConfigurationIdentityAddress}/{senderIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                        Environment.NewLine +
                                                        $"Expected: {string.Join(", ", recipientsRelationshipIds.Select(c => $"{c.RecipientIdentityAddress}/{c.RecipientPoolAlias}"))} " +
                                                        Environment.NewLine +
                                                        $"Actual: {string.Join(", ", recipientIdentityIds.Select(c => $"{c.ConfigurationIdentityAddress}/{c.PoolAlias}"))}");
                }

                foreach (var recipientIdentity in recipientIdentities)
                {
                    var numSentMessages = recipientsRelationshipIds
                        .First(c =>
                            c.RecipientIdentityAddress == recipientIdentity.ConfigurationIdentityAddress &&
                            c.RecipientPoolAlias == recipientIdentity.PoolAlias).NumberOfSentMessages;

                    for (var i = 0; i < numSentMessages; i++)
                    {
                        var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, senderIdentity.UserCredentials);

                        if (recipientIdentity.IdentityAddress is null)
                        {
                            throw new InvalidOperationException("Recipient identity address is null." +
                                                                Environment.NewLine +
                                                                $"sender-identity: {senderIdentity.IdentityAddress}/{senderIdentity.ConfigurationIdentityAddress}/{senderIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                                Environment.NewLine +
                                                                $"recipient-identity: null/{recipientIdentity.ConfigurationIdentityAddress}/{recipientIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}");
                        }

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
                            throw new InvalidOperationException("Failed to send message." +
                                                                Environment.NewLine +
                                                                $"sender-identity: {senderIdentity.IdentityAddress}/{senderIdentity.ConfigurationIdentityAddress}/{senderIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                                Environment.NewLine +
                                                                $"recipient-identity: {recipientIdentity.IdentityAddress}/{recipientIdentity.ConfigurationIdentityAddress}/{recipientIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                                Environment.NewLine +
                                                                $"Error Id: {messageResponse.Error.Id}" +
                                                                Environment.NewLine +
                                                                $"Error Code: {messageResponse.Error.Code}" +
                                                                Environment.NewLine +
                                                                $"Error Message: {messageResponse.Error.Message}");
                        }

                        if (messageResponse.Result is null) continue;

                        senderIdentity.SentMessages.Add((messageResponse.Result.Id, recipientIdentity));
                    }
                }
            }

            return Unit.Value;
        }
    }
}
