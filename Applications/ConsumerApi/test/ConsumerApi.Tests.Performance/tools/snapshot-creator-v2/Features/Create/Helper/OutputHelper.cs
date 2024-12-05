using System.Text;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public class OutputHelper : IOutputHelper
{
    public async Task WriteIdentities(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("IdentityAddress;ConfigurationIdentityAddress;PoolAlias;IdentityPoolType;DeviceId;Username;Password");

        foreach (var identity in identities)
        {
            foreach (var deviceId in identity.DeviceIds)
            {
                stringBuilder.AppendLine(
                    $"""{identity.IdentityAddress};{identity.ConfigurationIdentityAddress};{identity.PoolAlias};{identity.IdentityPoolType};{deviceId};{identity.UserCredentials.Username};"{identity.UserCredentials.Password}" """);
            }
        }

        var filePath = Path.Combine(outputDirName, "identities.csv");

        if (!Directory.Exists(outputDirName))
        {
            Directory.CreateDirectory(outputDirName);
        }

        var content = stringBuilder.ToString();
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task WriteRelationshipTemplates(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("IdentityAddress;ConfigurationIdentityAddress;PoolAlias;IdentityPoolType;RelationshipTemplateId;Used");

        var identitiesWithRelationshipTemplates = identities.Where(i => i.RelationshipTemplates.Count > 0);

        foreach (var identity in identitiesWithRelationshipTemplates)
        {
            foreach (var relationshipTemplateBag in identity.RelationshipTemplates)
            {
                stringBuilder.AppendLine(
                    $"{identity.IdentityAddress};{identity.ConfigurationIdentityAddress};{identity.PoolAlias};{identity.IdentityPoolType};{relationshipTemplateBag.Template.Id};{relationshipTemplateBag.Used}");
            }
        }

        var filePath = Path.Combine(outputDirName, "relationship_templates.csv");

        if (!Directory.Exists(outputDirName))
        {
            Directory.CreateDirectory(outputDirName);
        }

        var content = stringBuilder.ToString();
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task WriteRelationships(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(
            "RelationshipId;IdentityAddressFrom;ConfigurationIdentityAddressFrom;PoolAliasFrom;IdentityPoolTypeFrom;IdentityAddressTo;ConfigurationIdentityAddressTo;PoolAliasTo;IdentityPoolTypeTo");

        var identitiesWithRelationships = identities.Where(identity => identity.EstablishedRelationshipsById.Count > 0);

        foreach (var identity in identitiesWithRelationships)
        {
            foreach (var relatedIdentity in identity.EstablishedRelationshipsById)
            {
                stringBuilder.AppendLine(
                    $"{relatedIdentity.Key};{identity.IdentityAddress};{identity.ConfigurationIdentityAddress};{identity.PoolAlias};{identity.IdentityPoolType};" +
                    $"{relatedIdentity.Value.IdentityAddress};{relatedIdentity.Value.ConfigurationIdentityAddress};{relatedIdentity.Value.PoolAlias};{relatedIdentity.Value.IdentityPoolType}");
            }
        }

        var filePath = Path.Combine(outputDirName, "relationships.csv");

        if (!Directory.Exists(outputDirName))
        {
            Directory.CreateDirectory(outputDirName);
        }

        var content = stringBuilder.ToString();
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task WriteChallenges(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("CreatedByIdentityAddress;IdentityAddress;ConfigurationIdentityAddress;PoolAlias;IdentityPoolType;ChallengeId;CreatedByDevice");

        var identitiesWithChallenges = identities.Where(identity => identity.Challenges.Count > 0);

        foreach (var identity in identitiesWithChallenges)
        {
            foreach (var challenge in identity.Challenges)
            {
                stringBuilder.AppendLine(
                    $"{challenge.CreatedBy};{identity.IdentityAddress};{identity.ConfigurationIdentityAddress};{identity.PoolAlias};{identity.IdentityPoolType};{challenge.Id};{challenge.CreatedByDevice}");
            }
        }

        var filePath = Path.Combine(outputDirName, "challenges.csv");

        if (!Directory.Exists(outputDirName))
        {
            Directory.CreateDirectory(outputDirName);
        }

        var content = stringBuilder.ToString();
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task WriteMessages(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(
            "MessageId;IdentityAddressFrom;CreatedByDevice;ConfigurationIdentityAddressFrom;PoolAliasFrom;IdentityPoolTypeFrom;IdentityAddressTo;ConfigurationIdentityAddressTo;PoolAliasTo;IdentityPoolTypeTo");

        var identitiesWithSentMessages = identities.Where(identity => identity.SentMessages.Count > 0);

        foreach (var identity in identitiesWithSentMessages)
        {
            foreach (var (messageId, createdByDevice, recipient) in identity.SentMessages)
            {
                stringBuilder.AppendLine(
                    $"{messageId};{identity.IdentityAddress};{createdByDevice};{identity.ConfigurationIdentityAddress};{identity.PoolAlias};{identity.IdentityPoolType};" +
                    $"{recipient.IdentityAddress};{recipient.ConfigurationIdentityAddress};{recipient.PoolAlias};{recipient.IdentityPoolType}");
            }
        }

        var filePath = Path.Combine(outputDirName, "messages.csv");

        if (!Directory.Exists(outputDirName))
        {
            Directory.CreateDirectory(outputDirName);
        }

        var content = stringBuilder.ToString();
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task WriteDatawalletModifications(string outputDirName, List<DomainIdentity> identities)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("IdentityAddress;ConfigurationIdentityAddress;PoolAlias;IdentityPoolType;ModificationIndex;ModificationId");

        foreach (var identity in identities)
        {
            foreach (var modification in identity.DatawalletModifications)
            {
                stringBuilder.AppendLine($"{identity.IdentityAddress};{identity.ConfigurationIdentityAddress};{identity.PoolAlias};{identity.IdentityPoolType};{modification.Index};{modification.Id}");
            }
        }

        var filePath = Path.Combine(outputDirName, "datawallet_modifications.csv");

        if (!Directory.Exists(outputDirName))
        {
            Directory.CreateDirectory(outputDirName);
        }

        var content = stringBuilder.ToString();
        await File.WriteAllTextAsync(filePath, content);
    }
}
