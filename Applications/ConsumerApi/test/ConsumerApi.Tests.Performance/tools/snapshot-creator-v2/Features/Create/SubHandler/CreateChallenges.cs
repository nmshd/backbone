using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record CreateChallenges
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator
    public record CommandHandler : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithChallenges = request.Identities.Where(i => i.NumberOfChallenges > 0).ToList();


            foreach (var identitiesWithChallenge in identitiesWithChallenges)
            {
                var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identitiesWithChallenge.UserCredentials);

                for (var i = 0; i < identitiesWithChallenge.NumberOfChallenges; i++)
                {
                    var challenge = (await sdkClient.Challenges.CreateChallenge()).Result;

                    if (challenge is null) continue;

                    identitiesWithChallenge.Challenges.Add(challenge);
                }
            }

            return identitiesWithChallenges;
        }
    }
}
