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
                        senderIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress)
                    .Select(relationship =>
                    (
                        relationship.RecipientIdentityAddress,
                        relationship.RecipientPoolAlias
                    ))
                    .ToList();


                var recipientIdentities = request.Identities
                    .Where(recipient => recipientsRelationshipIds.Any(relationship =>
                        recipient.PoolAlias == relationship.RecipientPoolAlias &&
                        recipient.ConfigurationIdentityAddress == relationship.RecipientIdentityAddress))
                    .ToList();


                foreach (var recipientIdentity in recipientIdentities)
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

                    if (messageResponse.Result is null) continue;

                    senderIdentity.SentMessages.Add((messageResponse.Result.Id, recipientIdentity));
                }
            }

            return Unit.Value;
        }
    }
}
