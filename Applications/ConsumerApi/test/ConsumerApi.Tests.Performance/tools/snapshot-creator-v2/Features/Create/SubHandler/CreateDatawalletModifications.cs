using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateDatawalletModifications
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator

    public class CommandHandler(IDatawalletModificationFactory datawalletModificationFactory) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithDatawalletModifications = request.Identities
                .Where(i => i.NumberOfDatawalletModifications > 0)
                .ToArray();

            datawalletModificationFactory.TotalConfiguredDatawalletModifications = identitiesWithDatawalletModifications.Sum(i => i.NumberOfDatawalletModifications);

            var tasks = identitiesWithDatawalletModifications
                .Select(identityWithDatawalletModifications => datawalletModificationFactory.Create(request, identityWithDatawalletModifications))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
