using System.Diagnostics;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateRelationships
{
    public record Command(
        List<DomainIdentity> Identities,
        List<RelationshipAndMessages> RelationshipAndMessages,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CreateRelationships> Logger) : IRequestHandler<Command, Unit>
    {
        private int _numberOfCreatedRelationships;
        private int _totalRelationships;
        private readonly Lock _lockObj = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(Environment.ProcessorCount);
        private DomainIdentity[] _connectorIdentities = null!;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            _totalRelationships = request.RelationshipAndMessages.Count / 2;
            _numberOfCreatedRelationships = 0;

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
                .Select(appIdentity => ExecuteOuterCreateRelationships(request, appIdentity))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        private async Task ExecuteOuterCreateRelationships(Command request, DomainIdentity appIdentity)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var connectorIdentityToEstablishRelationshipWith = GetConnectorIdentitiesToEstablishRelationshipWith(request, appIdentity, _connectorIdentities);

                var appIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, appIdentity.UserCredentials);
                var connectorIdentitySdkClients = connectorIdentityToEstablishRelationshipWith
                    .ToDictionary(connectorIdentity => connectorIdentity,
                        connectorIdentity => Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, connectorIdentity.UserCredentials));


                var tasks = connectorIdentityToEstablishRelationshipWith
                    .Select(connectorIdentity => ExecuteInnerCreateRelationship(appIdentity, connectorIdentity, appIdentitySdkClient, connectorIdentitySdkClients[connectorIdentity]))
                    .ToArray();

                await Task.WhenAll(tasks);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task ExecuteInnerCreateRelationship(DomainIdentity appIdentity, DomainIdentity connectorIdentity, Client appIdentitySdkClient, Client connectorIdentitySdkClient)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            var acceptRelationshipResponse = await CreateRelationship(appIdentity, connectorIdentity, appIdentitySdkClient, connectorIdentitySdkClient);
            stopwatch.Stop();

            if (acceptRelationshipResponse.Result is null)
            {
                throw new InvalidOperationException(BuildErrorDetails($"Relationship was not created. {nameof(acceptRelationshipResponse)}.Result is null ",
                    appIdentity,
                    connectorIdentity));
            }

            using (_lockObj.EnterScope())
            {
                _numberOfCreatedRelationships++;
                appIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, connectorIdentity);
            }

            Logger.LogDebug("Created {CreatedRelationships}/{TotalRelationships} relationships. Relationship {RelationshipId} " +
                            "for App Identity {Address}/{ConfigurationAddress}/{Pool} " +
                            "with Connector Identity {ConnectorAddress}/{ConnectorConfigurationAddress}/{ConnectorPool} created in {ElapsedMilliseconds} ms",
                _numberOfCreatedRelationships,
                _totalRelationships,
                acceptRelationshipResponse.Result!.Id,
                appIdentity.IdentityAddress,
                appIdentity.ConfigurationIdentityAddress,
                appIdentity.PoolAlias,
                connectorIdentity.IdentityAddress,
                connectorIdentity.ConfigurationIdentityAddress,
                connectorIdentity.PoolAlias,
                stopwatch.ElapsedMilliseconds);
        }

        private static async Task<ApiResponse<RelationshipMetadata>> CreateRelationship(DomainIdentity appIdentity, DomainIdentity connectorIdentity, Client appIdentitySdkClient,
            Client connectorIdentitySdkClient)
        {
            var nextRelationshipTemplate = connectorIdentity.RelationshipTemplates.FirstOrDefault(t => t.Used == false);

            if (nextRelationshipTemplate == default)
            {
                throw new InvalidOperationException(BuildErrorDetails("Connector Identity has no further RelationshipTemplates.", appIdentity, connectorIdentity));
            }

            nextRelationshipTemplate.Used = true;

            var createRelationshipResponse = await appIdentitySdkClient.Relationships.CreateRelationship(
                new CreateRelationshipRequest
                {
                    RelationshipTemplateId = nextRelationshipTemplate.Template.Id,
                    Content = []
                });

            if (createRelationshipResponse.IsError)
            {
                throw new InvalidOperationException(BuildErrorDetails($"Failed to create relationship. {nameof(createRelationshipResponse)}.IsError.",
                    appIdentity,
                    connectorIdentity,
                    createRelationshipResponse));
            }

            if (createRelationshipResponse.Result is null)
            {
                throw new InvalidOperationException(BuildErrorDetails($"Relationship was not created. {nameof(createRelationshipResponse)}.Result is null ",
                    appIdentity,
                    connectorIdentity));
            }

            var acceptRelationshipResponse = await connectorIdentitySdkClient.Relationships.AcceptRelationship(
                createRelationshipResponse.Result!.Id,
                new AcceptRelationshipRequest());

            return acceptRelationshipResponse.IsError
                ? throw new InvalidOperationException(BuildErrorDetails($"Failed to accept relationship. {nameof(acceptRelationshipResponse)}.IsError.",
                    appIdentity,
                    connectorIdentity,
                    acceptRelationshipResponse))
                : acceptRelationshipResponse;
        }

        private static DomainIdentity[] GetConnectorIdentitiesToEstablishRelationshipWith(
            Command request,
            DomainIdentity appIdentity,
            DomainIdentity[] connectorIdentities)
        {
            var connectorRecipientIds = request.RelationshipAndMessages
                .Where(relationship =>
                    appIdentity.PoolAlias == relationship.SenderPoolAlias &&
                    appIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress)
                .Select(relationship => new RelationshipIdBag(
                    relationship.RecipientIdentityAddress,
                    relationship.RecipientPoolAlias))
                .OrderBy(relationshipIdBag => relationshipIdBag.PoolAlias)
                .ThenBy(relationshipIdBag => relationshipIdBag.IdentityAddress)
                .ToArray();

            var connectorIdentityToEstablishRelationshipWith = connectorIdentities
                .Where(connectorIdentity => connectorRecipientIds.Any(relationship =>
                    connectorIdentity.PoolAlias == relationship.PoolAlias &&
                    connectorIdentity.ConfigurationIdentityAddress == relationship.IdentityAddress))
                .ToArray();

            var connectorIdentityToEstablishRelationshipWithIds = connectorIdentityToEstablishRelationshipWith
                .Select(connectorIdentity => new RelationshipIdBag(
                    connectorIdentity.ConfigurationIdentityAddress,
                    connectorIdentity.PoolAlias))
                .OrderBy(relationshipIdBag => relationshipIdBag.PoolAlias)
                .ThenBy(relationshipIdBag => relationshipIdBag.IdentityAddress)
                .ToArray();

            return !connectorRecipientIds.SequenceEqual(connectorIdentityToEstablishRelationshipWithIds)
                ? throw new InvalidOperationException(BuildRelationshipErrorDetails("Mismatch between configured relationships and connector identities.",
                    appIdentity,
                    connectorRecipientIds,
                    connectorIdentityToEstablishRelationshipWithIds))
                : connectorIdentityToEstablishRelationshipWith;
        }
    }
}
