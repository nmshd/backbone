using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateIdentities
{
    public record Command(
        List<IdentityPoolConfiguration> IdentityPoolConfigurations,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 

    public record CommandHandler(ICreateIdentityCommand IdentityCommand) : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var identityConfigurations = request.IdentityPoolConfigurations
                .SelectMany(identityPoolConfiguration => identityPoolConfiguration.Identities)
                .ToArray();

            IdentityCommand.TotalIdentities = identityConfigurations.Length;

            var tasks = identityConfigurations
                .Select(identityConfiguration => IdentityCommand.CreateIdentity(request, identityConfiguration))
                .ToArray();

            var identities = await Task.WhenAll(tasks);

            return [.. identities];
        }
    }
}
