using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.Printer;
public class Printer : IPrinter
{
    public void PrintRelationships(IList<PoolEntry> pools, bool summaryOnly = false)
    {
        Console.WriteLine($"{pools.Where(p => p.IsApp()).SelectMany(p => p.Identities).Sum(i => i.IdentitiesToEstablishRelationshipsWith.Count)} relationships found");

        if (!summaryOnly)
            foreach (var appPoolIdentity in pools.Where(p => p.IsApp()).SelectMany(p => p.Identities))
            {
                Console.WriteLine($"Identity {appPoolIdentity.Nickname} of type App establishes {appPoolIdentity.IdentitiesToEstablishRelationshipsWith.Count} relationships:");
                foreach (var relatedIdentity in appPoolIdentity.IdentitiesToEstablishRelationshipsWith)
                    Console.WriteLine($"\t- with connector {relatedIdentity.Nickname}");
            }
    }

    public void PrintMessages(IList<PoolEntry> pools, bool summaryOnly = false)
    {
        var appIdentities = pools.Where(p => p.IsApp()).SelectMany(p => p.Identities).ToList();
        Console.WriteLine($"{appIdentities.Sum(i => i.IdentitiesToSendMessagesTo.Count)} messages found from App identities:");
        Console.WriteLine($"\t- {appIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("a"))} to app identities");
        Console.WriteLine($"\t- {appIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("c"))} to connector identities");
        Console.WriteLine($"\t- {appIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => !i.Nickname.StartsWith("c") && !i.Nickname.StartsWith("a"))} to other identities.");

        var connectorIdentities = pools.Where(p => p.IsApp()).SelectMany(p => p.Identities).ToList();
        Console.WriteLine($"{connectorIdentities.Sum(i => i.IdentitiesToSendMessagesTo.Count)} messages found from Connector identities:");
        Console.WriteLine($"\t- {connectorIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("a"))} to app identities");
        Console.WriteLine($"\t- {connectorIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => i.Nickname.StartsWith("c"))} to connector identities");
        Console.WriteLine($"\t- {connectorIdentities.SelectMany(s => s.IdentitiesToSendMessagesTo).Count(i => !i.Nickname.StartsWith("c") && !i.Nickname.StartsWith("a"))} to other identities.");
    }
}

public interface IPrinter

{
    protected internal void PrintRelationships(IList<PoolEntry> pools, bool summaryOnly = false);
    protected internal void PrintMessages(IList<PoolEntry> pools, bool summaryOnly = false);
}


