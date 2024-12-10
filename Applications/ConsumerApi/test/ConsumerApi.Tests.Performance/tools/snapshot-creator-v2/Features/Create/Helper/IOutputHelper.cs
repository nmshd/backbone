using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public interface IOutputHelper
{
    Task WriteIdentities(string outputDirName, List<DomainIdentity> identities);
    Task WriteRelationshipTemplates(string outputDirName, List<DomainIdentity> identities);
    Task WriteRelationships(string outputDirName, List<DomainIdentity> identities);
    Task WriteChallenges(string outputDirName, List<DomainIdentity> identities);
    Task WriteMessages(string outputDirName, List<DomainIdentity> identities);
    Task WriteDatawalletModifications(string outputDirName, List<DomainIdentity> identities);
}
