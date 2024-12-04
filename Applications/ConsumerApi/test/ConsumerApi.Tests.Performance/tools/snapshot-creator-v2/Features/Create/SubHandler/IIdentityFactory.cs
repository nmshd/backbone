using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public interface IIdentityFactory
{
    Task<DomainIdentity> Create(CreateIdentities.Command request, IdentityConfiguration identityConfiguration);
    int TotalIdentities { get; set; }
}
