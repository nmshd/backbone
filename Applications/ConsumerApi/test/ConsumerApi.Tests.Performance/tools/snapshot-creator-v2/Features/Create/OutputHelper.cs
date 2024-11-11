using System.Text;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public class OutputHelper : IOutputHelper
{
    public void WriteIdentities(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Address;DeviceId;Username;Password;Alias");

        foreach (var pool in identities)
        {
            foreach (var identity in identities)
            {
                foreach (var deviceId in identity.DeviceIds)
                {
                    stringBuilder.AppendLine($"""{identity.IdentityAddress};{deviceId};{identity.UserCredentials.Username};"{identity.UserCredentials.Password}";{pool.PoolAlias}""");
                }
            }
        }

        File.WriteAllTextAsync($@"{outputDirName}\identities.csv", stringBuilder.ToString());
    }

    public void WriteRelationshipTemplates(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("RelationshipId;AddressFrom;AddressTo");

        foreach (var identity in identities)
        {
            foreach (var relatedIdentity in identity.EstablishedRelationshipsById)
            {
                stringBuilder.AppendLine($"{relatedIdentity.Key};{identity.IdentityAddress};{relatedIdentity.Value.IdentityAddress}");
            }
        }

        File.WriteAllTextAsync($@"{outputDirName}\relationships.csv", stringBuilder.ToString());
    }

    public void WriteRelationships(string outputDirName, List<DomainIdentity> identities)
    {
        throw new NotImplementedException();
    }

    public void WriteChallenges(string outputDirName, List<DomainIdentity> identities)
    {
        throw new NotImplementedException();
    }

    public void WriteMessages(string outputDirName, List<DomainIdentity> identities)
    {
        throw new NotImplementedException();
    }

    public void WriteDatawalletModifications(string outputDirName, List<DomainIdentity> identities)
    {
        throw new NotImplementedException();
    }
}
