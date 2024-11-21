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
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator
    public record CommandHandler(ILogger<CreateChallenges> Logger) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithChallenges = request.Identities.Where(i => i.NumberOfChallenges > 0).ToList();

            var totalChallenges = identitiesWithChallenges.Sum(i => i.NumberOfChallenges);
            var numberOfCreatedChallenges = 0;

            foreach (var identityWithChallenge in identitiesWithChallenges)
            {
                Stopwatch stopwatch = new();

                stopwatch.Start();
                var challenges = await CreateChallenges(request, identityWithChallenge);
                stopwatch.Stop();

                numberOfCreatedChallenges += challenges.Count;
                Logger.LogDebug("Created {CreatedChallenges}/{TotalChallenges} challenges. Challenges of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                    numberOfCreatedChallenges,
                    totalChallenges,
                    identityWithChallenge.IdentityAddress,
                    identityWithChallenge.ConfigurationIdentityAddress,
                    identityWithChallenge.PoolAlias,
                    stopwatch.ElapsedMilliseconds);
            }

            return Unit.Value;
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
