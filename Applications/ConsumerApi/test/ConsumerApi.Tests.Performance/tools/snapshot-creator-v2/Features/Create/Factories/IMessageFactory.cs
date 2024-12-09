using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public interface IMessageFactory
{
    Task Create(CreateMessages.Command request, DomainIdentity senderIdentity);
    long TotalConfiguredMessages { get; set; }
}
