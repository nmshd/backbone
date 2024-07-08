using System.Text;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Tooling;

namespace Backbone.Identity.Pool.Creator.Application.Printer;
public class Printer : IPrinter
{
    public void PrintRelationships(IList<PoolEntry> pools, bool summaryOnly = false)
    {
        Console.WriteLine($"{pools.ExpectedNumberOfRelationships()} relationships expected.");
        Console.WriteLine($"{pools.Where(p => p.IsApp()).SelectMany(p => p.Identities).Sum(i => i.IdentitiesToEstablishRelationshipsWith.Count)} relationships found");

        if (summaryOnly) return;


        foreach (var appPoolIdentity in pools.Where(p => p.IsApp()).SelectMany(p => p.Identities))
        {
            Console.WriteLine($"Identity {appPoolIdentity.Nickname} of type App establishes {appPoolIdentity.IdentitiesToEstablishRelationshipsWith.Count} (of {appPoolIdentity.Pool.NumberOfRelationships}) relationships:");
            foreach (var relatedIdentity in appPoolIdentity.IdentitiesToEstablishRelationshipsWith)
                Console.WriteLine($"\t- with connector {relatedIdentity}");
        }

    }

    public void PrintMessages(IList<PoolEntry> pools, bool summaryOnly = false)
    {
        Console.WriteLine($"{pools.ExpectedNumberOfSentMessages()} messages expected.");
        Console.WriteLine($"{pools.NumberOfSentMessages()} messages sent.");

        var appIdentities = pools.Where(p => p.IsApp()).SelectMany(p => p.Identities).ToList();
        Console.WriteLine($"{appIdentities.Sum(i => i.IdentitiesToSendMessagesTo.Count)} messages found from App identities:");
        Console.WriteLine($"\t- {appIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("a"))} to app identities");
        Console.WriteLine($"\t- {appIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("c"))} to connector identities");
        Console.WriteLine($"\t- {appIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => !i.Nickname.StartsWith("c") && !i.Nickname.StartsWith("a"))} to other identities.");

        var connectorIdentities = pools.Where(p => p.IsConnector()).SelectMany(p => p.Identities).ToList();
        Console.WriteLine($"{connectorIdentities.Sum(i => i.IdentitiesToSendMessagesTo.Count)} messages found from Connector identities:");
        Console.WriteLine($"\t- {connectorIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("a"))} to app identities");
        Console.WriteLine($"\t- {connectorIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("c"))} to connector identities");
        Console.WriteLine($"\t- {connectorIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => !i.Nickname.StartsWith("c") && !i.Nickname.StartsWith("a"))} to other identities.");

        if (summaryOnly)
            return;

        foreach (var identity in connectorIdentities.Union(appIdentities))
        {
            Console.WriteLine($"Identity {identity} sends {identity.IdentitiesToSendMessagesTo.Count} messages (used {identity.Pool.NumberOfSentMessages - identity.SentMessagesCapacity} of {identity.Pool.NumberOfSentMessages}):");
            foreach (var recipientIdentity in identity.IdentitiesToSendMessagesTo)
            {
                Console.WriteLine($"\t - {recipientIdentity} (used {recipientIdentity.Pool.NumberOfReceivedMessages - recipientIdentity.ReceivedMessagesCapacity} of {recipientIdentity.Pool.NumberOfReceivedMessages})");
            }
        }
    }

    public void OutputAll(IList<PoolEntry> pools)
    {
        var outputDirName = $@"{GetProjectPath()}\poolCreator.{SystemTime.UtcNow:yyyyMMdd-HHmmss}";
        Directory.CreateDirectory(outputDirName);

        OutputIdentities(outputDirName, pools);
        //OutputRelationshipsAndMessages(outputDirName, pools);
    }

    private void OutputIdentities(string outputDirName, IList<PoolEntry> pools)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Address;DeviceId;Username;Password;Alias");
        foreach (var pool in pools)
        {
            foreach (var identity in pool.Identities)
            {
                foreach (var deviceId in identity.DeviceIds)
                {
                    stringBuilder.AppendLine($"""{identity.Address};{deviceId};{identity.UserCredentials.Username};"{identity.UserCredentials.Password}";{pool.Alias}""");
                }
            }
        }
        File.WriteAllTextAsync($@"{outputDirName}\identities.csv", stringBuilder.ToString());
    }

    private static string GetProjectPath()
    {
        var dir = Path.GetFullPath(@"..\..\..");
        return dir;
    }

    public void PrintString(string value, string filename)
    {
        var outputDirName = $@"{GetProjectPath()}\poolCreator.{SystemTime.UtcNow:yyyyMMdd-HHmmss}";
        Directory.CreateDirectory(outputDirName);
        File.WriteAllTextAsync($@"{outputDirName}\{filename}.csv", value);
    }
}

public interface IPrinter

{
    protected internal void PrintRelationships(IList<PoolEntry> pools, bool summaryOnly = false);
    protected internal void PrintMessages(IList<PoolEntry> pools, bool summaryOnly = false);
    void PrintString(string value, string filename);
}


