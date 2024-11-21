using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateRelationshipTemplates
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CreateRelationshipTemplates> Logger) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithRelationshipTemplates = request.Identities
                .Where(i => i.NumberOfRelationshipTemplates > 0)
                .ToList();

            var totalRelationshipTemplates = identitiesWithRelationshipTemplates.Sum(i => i.NumberOfRelationshipTemplates);
            var numberOfCreatedRelationshipTemplates = 0;

            foreach (var identity in identitiesWithRelationshipTemplates)
            {
                Stopwatch stopwatch = new();

                stopwatch.Start();
                var createdRelationshipTemplates = await CreateRelationshipTemplates(request, identity);
                stopwatch.Stop();

                numberOfCreatedRelationshipTemplates += createdRelationshipTemplates.Count;
                Logger.LogDebug(
                    "Created {CreatedRelationshipTemplates}/{TotalRelationshipTemplates} relationship templates.Relationship templates of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                    numberOfCreatedRelationshipTemplates,
                    totalRelationshipTemplates,
                    identity.IdentityAddress,
                    identity.ConfigurationIdentityAddress,
                    identity.PoolAlias,
                    stopwatch.ElapsedMilliseconds);
            }

            return Unit.Value;
        }

        private static async Task<List<RelationshipTemplateBag>> CreateRelationshipTemplates(Command request, DomainIdentity identity)
        {
            List<RelationshipTemplateBag> relationshipTemplates = [];

            var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials);

            for (var i = 0; i < identity.NumberOfRelationshipTemplates; i++)
            {
                var relationshipTemplateResponse = await sdkClient.RelationshipTemplates.CreateTemplate(
                    new CreateRelationshipTemplateRequest
                    {
                        Content = [],
                        ExpiresAt = DateTime.Now.EndOfYear(),
                        MaxNumberOfAllocations = 1000
                    });

                if (relationshipTemplateResponse.IsError)
                {
                    throw new InvalidOperationException(BuildErrorDetails("Failed to create relationship template.",
                        identity,
                        relationshipTemplateResponse));
                }

                if (relationshipTemplateResponse.Result is null) continue;

                var relationshipTemplateBag = new RelationshipTemplateBag(relationshipTemplateResponse.Result, false);
                relationshipTemplates.Add(relationshipTemplateBag);
            }

            identity.RelationshipTemplates.AddRange(relationshipTemplates);

            return relationshipTemplates;
        }
    }
}
