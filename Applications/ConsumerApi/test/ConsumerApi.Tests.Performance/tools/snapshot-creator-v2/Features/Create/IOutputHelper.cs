using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public interface IOutputHelper
{
    void WriteIdentities(string outputDirName, List<DomainIdentity> identities);
    void WriteRelationshipTemplates(string outputDirName, List<DomainIdentity> identities);
    void WriteRelationships(string outputDirName, List<DomainIdentity> identities);
    void WriteChallenges(string outputDirName, List<DomainIdentity> identities);
    void WriteMessages(string outputDirName, List<DomainIdentity> identities);
    void WriteDatawalletModifications(string outputDirName, List<DomainIdentity> identities);
}
