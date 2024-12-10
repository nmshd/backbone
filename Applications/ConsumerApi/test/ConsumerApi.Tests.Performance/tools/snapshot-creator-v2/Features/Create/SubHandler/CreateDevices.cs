using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract class CreateDevices
{
    public record Command(List<DomainIdentity> Identities, string BaseUrlAddress, ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public class CommandHandler(IDeviceFactory deviceFactory) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            deviceFactory.TotalConfiguredDevices = request.Identities.Sum(i => i.NumberOfDevices);

            var tasks = request.Identities
                .Select(identity => deviceFactory.Create(request, identity))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
