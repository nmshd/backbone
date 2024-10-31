using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;

public record AddDevices
{
    public record Command(List<DomainIdentity> Identities) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CommandHandler> Logger) : IRequestHandler<Command, List<DomainIdentity>>
    {
        public Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Adding devices ...");
            var identities = request.Identities;

            return Task.FromResult(identities);
        }
    }
}
