using System.Diagnostics;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Identity.Pool.Creator.Application.Checkers;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Tooling;
using Math = System.Math;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;

/// <summary>
/// We use a different approach here. Instead of creating all identities, then their relationships and lastly their messages,
/// instead we create an initial solution and use simulated annealing to search for better solutions.
///
/// This entails the creation of a lighter representation which can be used to quickly generate new solutions and check the score of the generated solution.
/// </summary>
public class SimulatedAnnealingPoolsGenerator
{
    private readonly string _baseAddress;
    private readonly ClientCredentials _clientCredentials;
    private readonly IPrinter _printer;

    private List<PoolEntry> Pools { get; }
    private List<Identity> Identities { get; }

    private readonly Dictionary<string, Identity> _identitiesDictionary;

    private const bool P = false;

    public SimulatedAnnealingPoolsGenerator(
        string baseAddress,
        string clientId,
        string clientSecret,
        PoolFileRoot configuration,
        IPrinter printer)
    {
        _baseAddress = baseAddress;
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _printer = printer;
        Pools = configuration.Pools.ToList();

        CreateIdentities();

        _localRandom = new Random();
        Identities = Pools.SelectMany(p => p.Identities).ToList();
        _identitiesDictionary = Identities.ToDictionary(i => i.Address);
    }

    private readonly Random _localRandom;

    public async Task CreatePools()
    {
        Generate();

        var (success, messages) = Pools.CheckSolution();
        if (!success)
        {
            Console.WriteLine("Solution validation failed.");

            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
        }

        _printer.PrintRelationships(Pools, summaryOnly: false);
        _printer.PrintMessages(Pools, summaryOnly: false);
    }

    public void Generate(double initialTemperature = 1500d, ulong maxIterations = 500000)
    {
        var currentSolution = GenerateInitialSolution();

        var currentScore = CalculateCore(currentSolution);
        var temperature = initialTemperature;

        for (ulong i = 0; i < maxIterations; i++)
        {
            var nextSolution = GetNextState(currentSolution);
            if (nextSolution is null) continue;
            var nextScore = CalculateCore(nextSolution);

            if (P) Console.Write($"\n\tTemp: {temperature} Score:{currentScore}, next action: ");

            // Positive delta means the next solution is better than the previous
            var delta = nextScore - currentScore;

            if (delta > 0)
            {
                currentScore = nextScore;
                currentSolution = nextSolution;
                if (P) Console.WriteLine(" - accepted due to delta");
            }
            else
            {
                var probability = delta == 0 ? 0 : Math.Exp(delta / temperature);
                var randomProbabilityAcceptance = _localRandom.NextDouble();

                if (probability > randomProbabilityAcceptance)
                {
                    currentScore = nextScore;
                    currentSolution = nextSolution;
                    if (P) Console.WriteLine(" - accepted due to probability");
                }
                else
                {
                    if (P) Console.WriteLine(" - rejected");
                }
            }

            temperature = initialTemperature * Math.Pow(1 / initialTemperature, i / (double)maxIterations);
        }

        currentSolution.Print(_identitiesDictionary);
    }

    private SolutionRepresentation? GetNextState(SolutionRepresentation currentSolution)
    {
        if (P) Console.Write("Next move is: ");
        if (currentSolution.Clone() is not SolutionRepresentation solution || solution.GetType() != typeof(SolutionRepresentation))
            return null;

        if (_localRandom.NextBoolean())
        {
            // Will mess with messages
            if (_localRandom.NextBoolean())
            {
                // will remove a message
                solution.RemoveRandomMessage();
                if (P) Console.Write("remove message");
            }
            else
            {
                // will add a message
                solution.SendMessage(_localRandom.GetRandomElement(Identities).Address, _localRandom.GetRandomElement(Identities).Address);
                if (P) Console.Write("add message");
            }
        }
        else
        {
            // Will mess with relationships
            if (_localRandom.NextBoolean())
            {
                // will remove a relationship
                solution.RemoveRandomRelationship();
                if (P) Console.Write("remove relationship");
            }
            else
            {
                // will add a relationship
                solution.EstablishRelationship(_localRandom.GetRandomElement(Identities).Address, _localRandom.GetRandomElement(Identities).Address);
                if (P) Console.Write("add relationship");
            }
        }

        return solution;
    }

    private long CalculateCore(SolutionRepresentation solution)
    {
        var relationshipsTarget = Pools.ExpectedNumberOfRelationships();
        var sentMessagesTarget = Pools.ExpectedNumberOfSentMessages();

        var validMessageCount = solution.GetNumberOfMessagesSentWithinRelationship();
        var invalidMessageCount = solution.GetNumberOfMessagesSentOutsideRelationship();

        var invalidRelationshipCount = solution.GetInvalidRelationshipCount(_identitiesDictionary);
        var validRelationshipCount = solution.GetRelationshipCount() - invalidRelationshipCount;

        return -10 * invalidMessageCount + 5 * validMessageCount +
               -8 * invalidRelationshipCount + 3 * validRelationshipCount;
    }

    private SolutionRepresentation GenerateInitialSolution()
    {
        var solution = new SolutionRepresentation();
        solution.EstablishRelationship(_localRandom.GetRandomElement(Identities).Address, _localRandom.GetRandomElement(Identities).Address);
        return solution;
    }

    private void CreateIdentities()
    {
        foreach (var poolEntry in Pools.Where(p => p.Amount > 0))
        {
            for (uint i = 0; i < poolEntry.Amount; i++)
            {
                poolEntry.Identities.Add(new Identity(
                        new UserCredentials("USR" + PasswordHelper.GeneratePassword(8, 8), PasswordHelper.GeneratePassword(18, 24)),
                        "ID1" + PasswordHelper.GeneratePassword(16, 16),
                        "DVC" + PasswordHelper.GeneratePassword(8, 8),
                        poolEntry,
                        i
                    )
                );
            }
        }
    }
}

public class SolutionRepresentation : ICloneable
{
    private Stopwatch CreatedAt = Stopwatch.StartNew();

    private ulong RelationshipCount = 0;
    private ulong MessagesCount = 0;

    private Dictionary<string, bool> Relationships { get; } = new();

    private Dictionary<string, IList<string>?> Messages { get; } = new();

    private readonly Random _localRandom = new();

    public bool EstablishRelationship(string identity1, string identity2)
    {
        var identityPairRepresentation = GetIdentityPairRepresentation(identity1, identity2);
        return EstablishRelationship(identityPairRepresentation);
    }

    public TimeSpan GetTimeSinceStart() => CreatedAt.Elapsed;
    public bool RemoveRelationship(string identity1, string identity2)
    {
        if (RelationshipCount <= 0)
            return false;

        var identityPairRepresentation = GetIdentityPairRepresentation(identity1, identity2);

        if (!Relationships.ContainsKey(identityPairRepresentation) || Relationships[identityPairRepresentation] == false)
            return false;

        Relationships[identityPairRepresentation] = false;

        RelationshipCount--;

        return true;
    }


    public bool SendMessage(string identityFrom, string identityTo)
    {
        //if (!Relationships.ContainsKey(GetIdentityPairRepresentation(identityFrom, identityTo)))
        //    return false;

        InsertIntoDictionaryOfLists(identityFrom, identityTo);

        MessagesCount++;

        return true;
    }

    public bool RemoveMessage(string identityFrom, string identityTo)
    {
        if (!Messages.ContainsKey(identityFrom) && !Messages[identityFrom]!.Contains(identityTo))
            return false;

        MessagesCount--;

        return Messages[identityFrom]!.Remove(identityTo);
    }

    private void InsertIntoDictionaryOfLists(string identityFrom, string identityTo)
    {
        if (!Messages.TryAdd(identityFrom, new List<string> { identityTo }))
        {
            Messages[identityFrom]!.Add(identityTo);
        }
    }

    private static string GetIdentityPairRepresentation(string identity1, string identity2)
    {
        return string.CompareOrdinal(identity1, identity2) > 0 ? identity2 + identity1 : identity1 + identity2;
    }

    private static (string, string) ReverseIdentityPairRepresentation(string identityPairRepresentation)
    {
        var length = identityPairRepresentation.Length;
        var half = length / 2;
        return (identityPairRepresentation[..half], identityPairRepresentation[half..]);
    }

    public long GetNumberOfSentMessages()
    {
        return Messages.Where(m => m.Value is not null).SelectMany(m => m.Value!).Count();
    }

    public long GetNumberOfMessagesSentWithinRelationship()
    {
        var res = 0;

        foreach (var (sender, recipients) in Messages)
        {
            if (recipients is null || recipients.Count == 0) continue;

            foreach (var recipient in recipients)
            {
                if (Relationships.TryGetValue(GetIdentityPairRepresentation(sender, recipient), out var hasRelationship) && hasRelationship)
                    res++;
            }
        }

        return res;
    }

    public long GetNumberOfMessagesSentOutsideRelationship()
    {
        var res = 0;

        foreach (var (sender, recipients) in Messages)
        {
            if (recipients is null || recipients.Count == 0) continue;

            foreach (var recipient in recipients)
            {
                if (!Relationships.TryGetValue(GetIdentityPairRepresentation(sender, recipient), out var hasRelationship) || !hasRelationship)
                    res++;
            }
        }

        return res;
    }

    public object Clone()
    {
        var ret = new SolutionRepresentation();

        foreach (var relationship in Relationships)
        {
            ret.EstablishRelationship(relationship.Key);
        }

        foreach (var message in Messages)
        {
            if (message.Value is null) continue;
            foreach (var recipient in message.Value)
            {
                ret.SendMessage(message.Key, recipient);
            }
        }

        return ret;
    }

    public void RemoveRandomMessage()
    {
        if (MessagesCount <= 0) return;

        var validMessages = Messages.Where(m => m.Value is not null && m.Value.Count > 0).ToList();
        var it = _localRandom.GetRandomElement(validMessages);
        RemoveMessage(it.Key, _localRandom.GetRandomElement(it.Value!));
    }

    public bool RemoveRandomRelationship()
    {
        if (RelationshipCount <= 0) return false;

        var establishedRelationships = Relationships.Where(r => r.Value).ToList();
        var it = _localRandom.GetRandomElement(establishedRelationships);
        return RemoveRelationship(it.Key);
    }

    private bool EstablishRelationship(string identityPairRepresentation)
    {
        if (Relationships.TryAdd(identityPairRepresentation, true))
        {
            RelationshipCount++;
            return true;
        }

        if (Relationships[identityPairRepresentation])
            return false;

        Relationships[identityPairRepresentation] = true;
        RelationshipCount++;
        return true;
    }


    private bool RemoveRelationship(string identityPairRepresentation)
    {
        if (RelationshipCount <= 0) return false;
        if (Relationships.TryAdd(identityPairRepresentation, false)) return true;

        if (Relationships[identityPairRepresentation] == false)
            return false;

        Relationships[identityPairRepresentation] = false;
        return true;
    }

    public long GetInvalidRelationshipCount(Dictionary<string, Identity> identities)
    {
        var res = 0;
        foreach (var (fromTo, _) in Relationships.Where(r => r.Value))
        {
            var (identity1, identity2) = ReverseIdentityPairRepresentation(fromTo);
            var i1Pool = identities[identity1].Pool;
            var i2Pool = identities[identity2].Pool;
            if (i1Pool.Type != i2Pool.Type &&
                (i1Pool.IsApp() || i1Pool.IsConnector()) &&
                (i2Pool.IsApp() || i2Pool.IsConnector())
                )
                res++;
        }

        return res;
    }

    public long GetRelationshipCount()
    {
        return Convert.ToInt64(RelationshipCount);
    }

    public void Print(Dictionary<string, Identity> identities)
    {
        Console.WriteLine(" ========= SOLUTION OUTPUT =========");
        Console.WriteLine($" =====> EXECUTION TIME: {GetTimeSinceStart()}");
        Console.WriteLine(" =====> ESTABLISHED RELATIONSHIPS");
        foreach (var (fromTo, _) in Relationships.Where((r) => r.Value))
        {
            var (identity1Address, identity2Address) = ReverseIdentityPairRepresentation(fromTo);
            var identity1 = identities[identity1Address];
            var identity2 = identities[identity2Address];

            Console.WriteLine($"{identity1} is related to {identity2}");
        }



    }
}

public static class RandomMethodExtensions
{
    public static bool NextBoolean(this Random random)
    {
        return random.NextDouble() <= 0.5;
    }

    public static T GetRandomElement<T>(this Random random, IList<T> list)
    {
        var randomElementIndex = Convert.ToInt32(random.NextInt64() % list.Count);
        return list[randomElementIndex];
    }

}
