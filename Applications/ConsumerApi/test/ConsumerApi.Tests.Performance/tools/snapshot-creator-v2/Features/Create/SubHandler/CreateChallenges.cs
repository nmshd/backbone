using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateChallenges
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator
    public class CommandHandler(IChallengeFactory challengeFactory) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithChallenges = request.Identities.Where(i => i.NumberOfChallenges > 0).ToArray();

            challengeFactory.TotalChallenges = identitiesWithChallenges.Sum(i => i.NumberOfChallenges);

            var tasks = identitiesWithChallenges
                .Select(identityWithChallenge => challengeFactory.Create(request, identityWithChallenge))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
