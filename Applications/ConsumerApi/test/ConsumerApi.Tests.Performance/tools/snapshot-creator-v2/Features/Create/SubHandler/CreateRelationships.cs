using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record CreateRelationships
{
    public record Command(List<DomainIdentity> Identities, List<RelationshipAndMessages> RelationshipAndMessages, string BaseUrlAddress, ClientCredentials ClientCredentials)
        : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var connectorIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.Connector).ToList();
            var appIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.App).ToList();


            if (connectorIdentities.Any(c => c.RelationshipTemplates.Count == 0))
                throw new Exception("One or more relationship target connector identities do not have a usable relationship template." +
                                    Environment.NewLine +
                                    "Connector identities:" +
                                    Environment.NewLine +
                                    $"{string.Join($",{Environment.NewLine}",
                                        connectorIdentities
                                            .Where(c => c.RelationshipTemplates.Count == 0)
                                            .Select(c => $"{c.IdentityAddress}/{c.ConfigurationIdentityAddress}/{c.PoolAlias} {IDENTITY_LOG_SUFFIX}"))}");


            foreach (var appIdentity in appIdentities)
            {
                var connectorRecipientIds = request.RelationshipAndMessages
                    .Where(relationship =>
                        appIdentity.PoolAlias == relationship.SenderPoolAlias &&
                        appIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress)
                    .Select(relationship =>
                    (
                        relationship.RecipientIdentityAddress,
                        relationship.RecipientPoolAlias
                    ))
                    .ToList();

                var connectorIdentityToEstablishRelationshipWith = connectorIdentities
                    .Where(connectorIdentity => connectorRecipientIds.Any(relationship =>
                        connectorIdentity.PoolAlias == relationship.RecipientPoolAlias &&
                        connectorIdentity.ConfigurationIdentityAddress == relationship.RecipientIdentityAddress))
                    .ToList();


                var connectorIdentityToEstablishRelationshipWithIds = connectorIdentityToEstablishRelationshipWith
                    .Select(c =>
                    (
                        c.ConfigurationIdentityAddress,
                        c.PoolAlias
                    ))
                    .ToList();

                if (connectorRecipientIds.Count != connectorIdentityToEstablishRelationshipWith.Count ||
                    !connectorRecipientIds.SequenceEqual(connectorIdentityToEstablishRelationshipWithIds))
                {
                    throw new InvalidOperationException("Mismatch between configured relationships and connector identities." +
                                                        Environment.NewLine +
                                                        $"app-identity: {appIdentity.IdentityAddress}/{appIdentity.ConfigurationIdentityAddress}/{appIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                        Environment.NewLine +
                                                        $"Expected: {string.Join(", ", connectorRecipientIds.Select(c => $"{c.RecipientIdentityAddress}/{c.RecipientPoolAlias}"))} " +
                                                        Environment.NewLine +
                                                        $"Actual: {string.Join(", ", connectorIdentityToEstablishRelationshipWithIds.Select(c => $"{c.ConfigurationIdentityAddress}/{c.PoolAlias}"))}");
                }

                foreach (var connectorIdentity in connectorIdentityToEstablishRelationshipWith)
                {
                    var appIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, appIdentity.UserCredentials);
                    var connectorIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, connectorIdentity.UserCredentials);

                    var nextRelationshipTemplate = connectorIdentity.RelationshipTemplates.FirstOrDefault(t => t.Used == false);

                    if (nextRelationshipTemplate == default)
                    {
                        throw new InvalidOperationException("Connector Identity has no further RelationshipTemplates." +
                                                            Environment.NewLine +
                                                            $"connector-identity: {connectorIdentity.IdentityAddress}/{connectorIdentity.ConfigurationIdentityAddress}/{connectorIdentity.IdentityPoolType} {IDENTITY_LOG_SUFFIX}");
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
                        throw new InvalidOperationException("Failed to create relationship." +
                                                            Environment.NewLine +
                                                            $"app-identity: {appIdentity.IdentityAddress}/{appIdentity.ConfigurationIdentityAddress}/{appIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                            Environment.NewLine +
                                                            $"connector-identity: {connectorIdentity.IdentityAddress}/{connectorIdentity.ConfigurationIdentityAddress}/{connectorIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                            Environment.NewLine +
                                                            $"Error Id: {createRelationshipResponse.Error.Id}" +
                                                            Environment.NewLine +
                                                            $"Error Code: {createRelationshipResponse.Error.Code}" +
                                                            Environment.NewLine +
                                                            $"Error Message: {createRelationshipResponse.Error.Message}");
                    }

                    var acceptRelationshipResponse = await connectorIdentitySdkClient.Relationships.AcceptRelationship(
                        createRelationshipResponse.Result!.Id,
                        new AcceptRelationshipRequest());

                    if (acceptRelationshipResponse.IsError)
                    {
                        throw new InvalidOperationException("Failed to accept relationship." +
                                                            Environment.NewLine +
                                                            $"app-identity: {appIdentity.IdentityAddress}/{appIdentity.ConfigurationIdentityAddress}/{appIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                            Environment.NewLine +
                                                            $"connector-identity: {connectorIdentity.IdentityAddress}/{connectorIdentity.ConfigurationIdentityAddress}/{connectorIdentity.PoolAlias} {IDENTITY_LOG_SUFFIX}" +
                                                            Environment.NewLine +
                                                            $"Error Id: {acceptRelationshipResponse.Error.Id}" +
                                                            Environment.NewLine +
                                                            $"Error Code: {acceptRelationshipResponse.Error.Code}" +
                                                            Environment.NewLine +
                                                            $"Error Message: {acceptRelationshipResponse.Error.Message}");
                    }

                    if (acceptRelationshipResponse.Result is null) continue;

                    appIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, connectorIdentity);
                }
            }

            return Unit.Value;
        }
    }
}
