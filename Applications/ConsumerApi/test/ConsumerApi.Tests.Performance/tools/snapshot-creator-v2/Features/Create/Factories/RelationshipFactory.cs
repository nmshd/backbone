using System.Diagnostics;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public class RelationshipFactory(ILogger<RelationshipFactory> logger, IConsumerApiHelper consumerApiHelper) : IRelationshipFactory
{
    internal int NumberOfCreatedRelationships;
    public int TotalRelationships { get; set; }
    private readonly Lock _lockObj = new();
    internal readonly SemaphoreSlim SemaphoreSlim = new(Environment.ProcessorCount);

    public async Task Create(CreateRelationships.Command request, DomainIdentity appIdentity, DomainIdentity[] connectorIdentities)
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            var connectorIdentityToEstablishRelationshipWith = GetConnectorIdentitiesToEstablishRelationshipWith(request, appIdentity, connectorIdentities);

            var appIdentitySdkClient = consumerApiHelper.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, appIdentity.UserCredentials);
            var connectorIdentitySdkClients = connectorIdentityToEstablishRelationshipWith
                .ToDictionary(connectorIdentity => connectorIdentity,
                    connectorIdentity => consumerApiHelper.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, connectorIdentity.UserCredentials));


            var tasks = connectorIdentityToEstablishRelationshipWith
                .Select(connectorIdentity => InnerCreate(appIdentity, connectorIdentity, appIdentitySdkClient, connectorIdentitySdkClients[connectorIdentity]))
                .ToArray();

            await Task.WhenAll(tasks);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private async Task InnerCreate(DomainIdentity appIdentity, DomainIdentity connectorIdentity, Client appIdentitySdkClient, Client connectorIdentitySdkClient)
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
            NumberOfCreatedRelationships++;
            appIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, connectorIdentity);
        }

        logger.LogDebug("Created {CreatedRelationships}/{TotalRelationships} relationships. Relationship {RelationshipId} " +
                        "for App Identity {Address}/{ConfigurationAddress}/{Pool} " +
                        "with Connector Identity {ConnectorAddress}/{ConnectorConfigurationAddress}/{ConnectorPool} created in {ElapsedMilliseconds} ms",
            NumberOfCreatedRelationships,
            TotalRelationships,
            acceptRelationshipResponse.Result!.Id,
            appIdentity.IdentityAddress,
            appIdentity.ConfigurationIdentityAddress,
            appIdentity.PoolAlias,
            connectorIdentity.IdentityAddress,
            connectorIdentity.ConfigurationIdentityAddress,
            connectorIdentity.PoolAlias,
            stopwatch.ElapsedMilliseconds);
    }

    private async Task<ApiResponse<RelationshipMetadata>> CreateRelationship(DomainIdentity appIdentity, DomainIdentity connectorIdentity, Client appIdentitySdkClient,
        Client connectorIdentitySdkClient)
    {
        var nextRelationshipTemplate = connectorIdentity.RelationshipTemplates.FirstOrDefault(t => t.Used == false);

        if (nextRelationshipTemplate == default)
        {
            throw new InvalidOperationException(BuildErrorDetails("Connector Identity has no further RelationshipTemplates.", appIdentity, connectorIdentity));
        }

        nextRelationshipTemplate.Used = true;

        var createRelationshipResponse = await consumerApiHelper.CreateRelationship(appIdentitySdkClient, nextRelationshipTemplate);

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

        var acceptRelationshipResponse = await consumerApiHelper.AcceptRelationship(connectorIdentitySdkClient, createRelationshipResponse);

        return acceptRelationshipResponse.IsError
            ? throw new InvalidOperationException(BuildErrorDetails($"Failed to accept relationship. {nameof(acceptRelationshipResponse)}.IsError.",
                appIdentity,
                connectorIdentity,
                acceptRelationshipResponse))
            : acceptRelationshipResponse;
    }

    private static DomainIdentity[] GetConnectorIdentitiesToEstablishRelationshipWith(
        CreateRelationships.Command request,
        DomainIdentity appIdentity,
        DomainIdentity[] connectorIdentities)

    {
        var connectorRecipientIds = request.RelationshipAndMessages
            .Where(relationship =>
                appIdentity.PoolAlias == relationship.SenderPoolAlias &&
                appIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress)
            .Select(relationship => new RelationshipIdentityBag(
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
            .Select(connectorIdentity => new RelationshipIdentityBag(
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
