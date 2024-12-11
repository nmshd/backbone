using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateMessages
{
    public record Command(
        List<DomainIdentity> Identities,
        List<RelationshipAndMessages> RelationshipAndMessages,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 

    public class CommandHandler(IMessageFactory messageFactory) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            messageFactory.TotalConfiguredMessages = request.RelationshipAndMessages.Sum(relationship => relationship.NumberOfSentMessages);

            var senderIdentities = request.Identities
                .Where(identity => request.RelationshipAndMessages.Any(relationship =>
                    identity.PoolAlias == relationship.SenderPoolAlias &&
                    identity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress &&
                    relationship.NumberOfSentMessages > 0))
                .ToArray();

            var sumOfAllMessages = senderIdentities.Sum(s => s.NumberOfSentMessages);
            if (sumOfAllMessages != messageFactory.TotalConfiguredMessages)
            {
                throw new InvalidOperationException(
                    $"Mismatch between configured relationships and connector identities. SenderIdentities.SumMessages: {sumOfAllMessages}, TotalMessages: {messageFactory.TotalConfiguredMessages}");
            }

            var tasks = senderIdentities.Select(senderIdentity => messageFactory.Create(request, senderIdentity)).ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
