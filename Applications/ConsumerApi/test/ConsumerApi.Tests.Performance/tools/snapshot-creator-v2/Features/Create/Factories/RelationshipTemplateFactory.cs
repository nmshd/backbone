using System.Diagnostics;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public class RelationshipTemplateFactory(ILogger<RelationshipTemplateFactory> logger, IConsumerApiHelper consumerApiHelper) : IRelationshipTemplateFactory
{
    internal int NumberOfCreatedRelationshipTemplates;
    public int TotalRelationshipTemplates { get; set; }

    private readonly Lock _lockObj = new();
    internal readonly SemaphoreSlim SemaphoreSlim = new(Environment.ProcessorCount);

    public async Task Create(CreateRelationshipTemplates.Command request, DomainIdentity identity)
    {
        await SemaphoreSlim.WaitAsync();

        try
        {
            Stopwatch stopwatch = new();

            stopwatch.Start();
            var createdRelationshipTemplates = await CreateRelationshipTemplates(request, identity);
            stopwatch.Stop();

            using (_lockObj.EnterScope())
            {
                NumberOfCreatedRelationshipTemplates += createdRelationshipTemplates.Count;
            }

            logger.LogDebug(
                "Created {CreatedRelationshipTemplates}/{TotalRelationshipTemplates} relationship templates.  Semaphore.Count: {SemaphoreCount} - Relationship templates of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                NumberOfCreatedRelationshipTemplates,
                TotalRelationshipTemplates,
                SemaphoreSlim.CurrentCount,
                identity.IdentityAddress,
                identity.ConfigurationIdentityAddress,
                identity.PoolAlias,
                stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private async Task<List<RelationshipTemplateBag>> CreateRelationshipTemplates(CreateRelationshipTemplates.Command request, DomainIdentity identity)
    {
        List<RelationshipTemplateBag> relationshipTemplates = [];

        var sdkClient = consumerApiHelper.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials);

        for (var i = 0; i < identity.NumberOfRelationshipTemplates; i++)
        {
            var relationshipTemplateResponse = await consumerApiHelper.CreateRelationshipTemplate(sdkClient);

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
