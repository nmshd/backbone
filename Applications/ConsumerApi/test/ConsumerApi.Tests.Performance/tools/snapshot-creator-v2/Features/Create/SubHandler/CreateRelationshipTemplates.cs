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
        CreateSnapshot.PerformanceLoadTest LoadTag,
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CreateRelationshipTemplates> Logger) : IRequestHandler<Command, Unit>
    {
        private int _numberOfCreatedRelationshipTemplates;
        private int _totalRelationshipTemplates;
        private readonly Lock _lockObj = new();
        private SemaphoreSlim _semaphoreSlim = null!;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithRelationshipTemplates = request.Identities
                .Where(i => i.NumberOfRelationshipTemplates > 0)
                .ToArray();

            _totalRelationshipTemplates = identitiesWithRelationshipTemplates.Sum(i => i.NumberOfRelationshipTemplates);
            _numberOfCreatedRelationshipTemplates = 0;


            var maxDegreeOfParallelism = request.LoadTag switch
            {
                CreateSnapshot.PerformanceLoadTest.Low => Environment.ProcessorCount,
                CreateSnapshot.PerformanceLoadTest.Medium => Environment.ProcessorCount,
                CreateSnapshot.PerformanceLoadTest.High => Environment.ProcessorCount / 2,
                _ => Environment.ProcessorCount / 2
            };

            _semaphoreSlim = new SemaphoreSlim(maxDegreeOfParallelism);

            var tasks = identitiesWithRelationshipTemplates
                .Select(identityWithRelationshipTemplate => ExecuteCreateRelationshipTemplates(request, identityWithRelationshipTemplate))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        private async Task ExecuteCreateRelationshipTemplates(Command request, DomainIdentity identity)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                Stopwatch stopwatch = new();

                stopwatch.Start();
                var createdRelationshipTemplates = await CreateRelationshipTemplates(request, identity);
                stopwatch.Stop();

                using (_lockObj.EnterScope())
                {
                    _numberOfCreatedRelationshipTemplates += createdRelationshipTemplates.Count;
                }

                Logger.LogDebug(
                    "Created {CreatedRelationshipTemplates}/{TotalRelationshipTemplates} relationship templates.  Semaphore.Count: {SemaphoreCount} - Relationship templates of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                    _numberOfCreatedRelationshipTemplates,
                    _totalRelationshipTemplates,
                    _semaphoreSlim.CurrentCount,
                    identity.IdentityAddress,
                    identity.ConfigurationIdentityAddress,
                    identity.PoolAlias,
                    stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
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
