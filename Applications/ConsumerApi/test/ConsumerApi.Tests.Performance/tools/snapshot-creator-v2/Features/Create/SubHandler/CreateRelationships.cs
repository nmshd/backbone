using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateRelationships
{
    public record Command(
        List<DomainIdentity> Identities,
        List<RelationshipAndMessages> RelationshipAndMessages,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 

    public class CommandHandler(IRelationshipFactory relationshipFactory) : IRequestHandler<Command, Unit>
    {
        private DomainIdentity[] _connectorIdentities = null!;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            relationshipFactory.TotalConfiguredRelationships = request.RelationshipAndMessages.Count / 2;

            _connectorIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.Connector).ToArray();
            var appIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.App).ToArray();

            if (_connectorIdentities.Any(c => c.RelationshipTemplates.Count == 0))
            {
                var materializedConnectorIdentities = _connectorIdentities
                    .Where(c => c.RelationshipTemplates.Count == 0)
                    .Select(c => $"{c.IdentityAddress}/{c.ConfigurationIdentityAddress}/{c.PoolAlias} {IDENTITY_LOG_SUFFIX}")
                    .ToArray();

                throw new InvalidOperationException("One or more relationship target connector identities do not have a usable relationship template." +
                                                    Environment.NewLine +
                                                    "Connector identities:" +
                                                    Environment.NewLine +
                                                    $"{string.Join($",{Environment.NewLine}", materializedConnectorIdentities)}");
            }

            var tasks = appIdentities
                .Select(appIdentity => relationshipFactory.Create(request, appIdentity, _connectorIdentities))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
