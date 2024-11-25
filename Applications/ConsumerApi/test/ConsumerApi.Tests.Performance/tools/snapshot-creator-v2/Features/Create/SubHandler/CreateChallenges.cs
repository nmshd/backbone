using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateChallenges
{
    public record Command(
        CreateSnapshot.PerformanceLoadTest LoadTag,
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator
    public record CommandHandler(ILogger<CreateChallenges> Logger) : IRequestHandler<Command, Unit>
    {
        private int _numberOfCreatedChallenges;
        private int _totalChallenges;
        private readonly Lock _lockObj = new();
        private SemaphoreSlim _semaphoreSlim = null!;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithChallenges = request.Identities.Where(i => i.NumberOfChallenges > 0).ToArray();

            _totalChallenges = identitiesWithChallenges.Sum(i => i.NumberOfChallenges);
            _numberOfCreatedChallenges = 0;

            var maxDegreeOfParallelism = request.LoadTag switch
            {
                CreateSnapshot.PerformanceLoadTest.Low => Environment.ProcessorCount,
                CreateSnapshot.PerformanceLoadTest.Medium => Environment.ProcessorCount,
                CreateSnapshot.PerformanceLoadTest.High => Environment.ProcessorCount / 2,
                _ => Environment.ProcessorCount / 2
            };

            _semaphoreSlim = new SemaphoreSlim(maxDegreeOfParallelism);

            var tasks = identitiesWithChallenges
                .Select(identityWithChallenge => ExecuteCreateChallenges(request, identityWithChallenge))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        private async Task ExecuteCreateChallenges(Command request, DomainIdentity identityWithChallenge)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                Stopwatch stopwatch = new();

                stopwatch.Start();
                var challenges = await CreateChallenges(request, identityWithChallenge);
                stopwatch.Stop();

                using (_lockObj.EnterScope())
                {
                    _numberOfCreatedChallenges += challenges.Count;
                }

                Logger.LogDebug(
                    "Created {CreatedChallenges}/{TotalChallenges} challenges.  Semaphore.Count: {SemaphoreCount} - Challenges of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                    _numberOfCreatedChallenges,
                    _totalChallenges,
                    _semaphoreSlim.CurrentCount,
                    identityWithChallenge.IdentityAddress,
                    identityWithChallenge.ConfigurationIdentityAddress,
                    identityWithChallenge.PoolAlias,
                    stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<List<Challenge>> CreateChallenges(Command request, DomainIdentity identityWithChallenge)
        {
            List<Challenge> challenges = [];
            var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identityWithChallenge.UserCredentials);

            for (var i = 0; i < identityWithChallenge.NumberOfChallenges; i++)
            {
                if (sdkClient.DeviceData?.DeviceId is null || identityWithChallenge.DeviceIds.Count == 0)
                {
                    var identityDeviceId = identityWithChallenge.DeviceIds.Count > 0 ? string.Join(',', identityWithChallenge.DeviceIds) : "null";

                    Logger.LogWarning("SDK Client DeviceId is {SdkClientDeviceId}! " +
                                      "Configuration {Address}/{ConfigurationAddress}/{Pool} \r\n" +
                                      "Identity DeviceIds: {IdentityDeviceIds}",
                        sdkClient.DeviceData?.DeviceId is null,
                        identityWithChallenge.IdentityAddress,
                        identityWithChallenge.ConfigurationIdentityAddress,
                        identityWithChallenge.PoolAlias,
                        identityDeviceId);
                }

                var apiResponse = await sdkClient.Challenges.CreateChallenge();

                if (apiResponse.IsError)
                {
                    throw new InvalidOperationException(BuildErrorDetails("Failed to create challenge.",
                        identityWithChallenge,
                        apiResponse));
                }

                var challenge = apiResponse.Result;

                if (challenge is null) continue;

                challenges.Add(challenge);
            }

            identityWithChallenge.Challenges.AddRange(challenges);

            return challenges;
        }
    }
}
