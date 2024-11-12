using System.Text;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public class OutputHelper : IOutputHelper
{
    public void WriteIdentities(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("IdentityAddress;DeviceId;Username;Password;ConfigurationIdentityAddress;PoolAlias");

        foreach (var pool in identities)
        {
            foreach (var identity in identities)
            {
                foreach (var deviceId in identity.DeviceIds)
                {
                    stringBuilder.AppendLine($"""
                                              {identity.IdentityAddress};
                                              {deviceId};
                                              {identity.UserCredentials.Username};
                                              "{identity.UserCredentials.Password}";
                                              {pool.ConfigurationIdentityAddress};
                                              {pool.PoolAlias}
                                              """);
                }
            }
        }

        var filePath = Path.Combine(outputDirName, "identities.csv");
        File.WriteAllTextAsync(filePath, stringBuilder.ToString());
    }

    public void WriteRelationshipTemplates(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("IdentityAddress;RelationshipTemplateId;Used");

        foreach (var identity in identities)
        {
            foreach (var (template, used) in identity.RelationshipTemplates)
            {
                stringBuilder.AppendLine($"{identity.IdentityAddress};{template.Id};{used}");
            }
        }

        var filePath = Path.Combine(outputDirName, "relationshipTemplates.csv");
        File.WriteAllTextAsync(filePath, stringBuilder.ToString());
    }

    public void WriteRelationships(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("RelationshipId;" +
                                 "IdentityAddressFrom;ConfigurationIdentityAddressFrom;PoolAliasFrom;" +
                                 "IdentityAddressTo;ConfigurationIdentityAddressTo;PoolAliasTo");

        foreach (var identity in identities)
        {
            foreach (var relatedIdentity in identity.EstablishedRelationshipsById)
            {
                stringBuilder.AppendLine($"{relatedIdentity.Key};" +
                                         $"{identity.IdentityAddress};" +
                                         $"{identity.ConfigurationIdentityAddress};" +
                                         $"{identity.PoolAlias};" +
                                         $"{relatedIdentity.Value.IdentityAddress};" +
                                         $"{relatedIdentity.Value.ConfigurationIdentityAddress};" +
                                         $"{relatedIdentity.Value.PoolAlias};");
            }
        }

        var filePath = Path.Combine(outputDirName, "relationships.csv");
        File.WriteAllTextAsync(filePath, stringBuilder.ToString());
    }

    public void WriteChallenges(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("CreatedByIdentityAddress;IdentityAddress;ConfigurationIdentityAddress;PoolAlias;ChallengeId;CreatedByDevice");

        var identitiesWithChallenges = identities.Where(identity => identity.Challenges.Count > 0);

        foreach (var identity in identitiesWithChallenges)
        {
            foreach (var challenge in identity.Challenges)
            {
                stringBuilder.AppendLine($"{challenge.CreatedBy};" +
                                         $"{identity.IdentityAddress};" +
                                         $"{identity.ConfigurationIdentityAddress};" +
                                         $"{identity.PoolAlias};" +
                                         $"{challenge.Id};" +
                                         $"{challenge.CreatedByDevice}");
            }
        }

        var filePath = Path.Combine(outputDirName, "challenges.csv");
        File.WriteAllTextAsync(filePath, stringBuilder.ToString());
    }

    public void WriteMessages(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("MessageId;" +
                                 "IdentityAddressFrom;ConfigurationIdentityAddressFrom;PoolAliasFrom;" +
                                 "IdentityAddressTo;ConfigurationIdentityAddressTo;PoolAliasTo");

        var identitiesWithSentMessages = identities.Where(identity => identity.SentMessages.Count > 0);

        foreach (var identity in identitiesWithSentMessages)
        {
            foreach (var (messageId, recipient) in identity.SentMessages)
            {
                stringBuilder.AppendLine($"{messageId};" +
                                         $"{identity.IdentityAddress};" +
                                         $"{identity.ConfigurationIdentityAddress};" +
                                         $"{identity.PoolAlias};" +
                                         $"{recipient.IdentityAddress};" +
                                         $"{recipient.ConfigurationIdentityAddress};" +
                                         $"{recipient.PoolAlias};");
            }
        }

        var filePath = Path.Combine(outputDirName, "messages.csv");
        File.WriteAllTextAsync(filePath, stringBuilder.ToString());
    }

    public void WriteDatawalletModifications(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("IdentityAddress;ConfigurationIdentityAddress;PoolAlias;ModificationIndex;ModificationId");

        foreach (var identity in identities)
        {
            foreach (var modification in identity.DatawalletModifications)
            {
                stringBuilder.AppendLine($"{identity.IdentityAddress};" +
                                         $"{identity.ConfigurationIdentityAddress};" +
                                         $"{identity.PoolAlias};" +
                                         $"{modification.Index};" +
                                         $"{modification.Id}");
            }
        }

        var filePath = Path.Combine(outputDirName, "datawalletModifications.csv");
        File.WriteAllTextAsync(filePath, stringBuilder.ToString());
    }
}
