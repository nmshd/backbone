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
            messageFactory.TotalMessages = request.RelationshipAndMessages.Sum(relationship => relationship.NumberOfSentMessages);

            var senderIdentities = request.Identities
                .Where(identity => request.RelationshipAndMessages.Any(relationship =>
                    identity.PoolAlias == relationship.SenderPoolAlias &&
                    identity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress &&
                    relationship.NumberOfSentMessages > 0))
                .ToArray();

            var sum = senderIdentities.Sum(s => s.NumberOfSentMessages);
            if (sum != messageFactory.TotalMessages)
            {
                throw new InvalidOperationException(
                    $"Mismatch between configured relationships and connector identities. SenderIdentities.SumMessages: {sum}, TotalMessages: {messageFactory.TotalMessages}");
            }

            var tasks = senderIdentities.Select(senderIdentity => messageFactory.Create(request, senderIdentity)).ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
