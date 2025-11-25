using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.Printer;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tools;
using Backbone.Tooling;
using GeneratorIdentity = Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Domain.Identity;
using Math = System.Math;

// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected
// -- used for debugging

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsGenerator;

/// <summary>
/// We use a different approach here. Instead of creating all identities, then their relationships and lastly their messages,
/// instead we create an initial solution and use simulated annealing to search for better solutions.
///
/// This entails the creation of a lighter representation which can be used to quickly generate new solutions and check the score of the generated solution.
/// </summary>
public class SimulatedAnnealingPoolsGenerator
{
    private readonly IPrinter _printer;

    private List<PoolEntry> Pools { get; }
    private List<GeneratorIdentity> Identities { get; }

    private readonly Dictionary<uint, GeneratorIdentity> _identitiesDictionary;
    private readonly Dictionary<uint, GeneratorIdentity> _appIdentitiesDictionary;
    private readonly Dictionary<uint, GeneratorIdentity> _connectorIdentitiesDictionary;

    private const bool PRINT_OUTPUT = false;

    public SimulatedAnnealingPoolsGenerator(
        PoolFileRoot configuration,
        IPrinter printer)
    {
        _printer = printer;
        Pools = configuration.Pools.ToList();

        if (!configuration.Pools.SelectMany(p => p.Identities).Any()) CreateIdentities();
        Pools.AddUonsIfMissing();

        _localRandom = new Random();
        Identities = Pools.SelectMany(p => p.Identities).ToList();
        _identitiesDictionary = Identities.ToDictionary(i => i.UniqueOrderNumber);
        _appIdentitiesDictionary = Identities.Where(i => i.Pool.IsApp()).ToDictionary(i => i.UniqueOrderNumber);
        _connectorIdentitiesDictionary = Identities.Where(i => i.Pool.IsConnector()).ToDictionary(i => i.UniqueOrderNumber);
        _connectorMessageRatio = configuration.Configuration.MessagesSentByConnectorRatio;
    }

    private readonly Random _localRandom;
    private readonly decimal _connectorMessageRatio;

    public void CreatePools()
    {
        _printer.PrintStringToFile(Generate().GetAsCSV(_identitiesDictionary), "relationshipsAndMessages");
    }

    private SolutionRepresentation Generate(double initialTemperature = 20d, ulong maxIterations = 20000)
    {
        var progress = new ProgressBar(Convert.ToInt64(maxIterations));
        var currentSolution = GenerateSolutionFromPools(Pools);

        var currentScore = CalculateScore(currentSolution, _identitiesDictionary);
        var temperature = initialTemperature;

        for (ulong i = 0; i < maxIterations; i++)
        {
            if (PRINT_OUTPUT)
                Console.Write($"Temp: {temperature:F3} Score:{currentScore}, solution m: {currentSolution.GetSentMessagesCount():D4}, r:{currentSolution.GetRelationshipCount():D4}. Next action: ");

            var solutions = new ConcurrentBag<SolutionRepresentation>();
            var forMax = Convert.ToUInt32(Environment.ProcessorCount) - 2;

            var solution = currentSolution;
            Parallel.For(0, forMax, _ =>
            {
                var nextSolution = solution.Clone() as SolutionRepresentation;
                for (uint si = 0; si < 6; si++)
                {
                    nextSolution = GetNextState(nextSolution!, _connectorMessageRatio, si, 6);
                    if (PRINT_OUTPUT && si != 6 - 1) Console.Write(", ");
                }

                if (nextSolution != null) solutions.Add(nextSolution);
            });
            var scoredSolutions = solutions.Select(s => new { Solution = s, Score = CalculateScore(s, _identitiesDictionary) }).OrderByDescending(s => s.Score).First();

            var nextSolution = scoredSolutions.Solution;
            var nextScore = scoredSolutions.Score;

            if (PRINT_OUTPUT) Console.Write($" Next score is {nextScore}");

            // Positive delta means the next solution is better than the previous
            var delta = nextScore - currentScore;

            if (delta > 0)
            {
                currentScore = nextScore;
                currentSolution = nextSolution;
                if (PRINT_OUTPUT) Console.WriteLine(" - accepted due to delta");
            }
            else
            {
                var probability = delta == 0 ? 0.4 : Math.Exp(delta / temperature);
                var randomProbabilityAcceptance = _localRandom.NextDouble();

                if (probability / 2 > randomProbabilityAcceptance)
                {
                    currentScore = nextScore;
                    currentSolution = nextSolution;
                    if (PRINT_OUTPUT) Console.WriteLine(" - accepted due to probability");
                }
                else
                {
                    if (PRINT_OUTPUT) Console.WriteLine(" - rejected due to probability");
                }
            }

            currentSolution.IterationCount = i;
            temperature = initialTemperature * Math.Pow(1 / initialTemperature, i / (double)maxIterations);
            progress.Increment();
        }

        currentSolution.Print(_identitiesDictionary, Pools);
        return currentSolution;
    }

    private static SolutionRepresentation GenerateSolutionFromPools(List<PoolEntry> pools)
    {
        var res = new SolutionRepresentation();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var pool in pools)
        {
            foreach (var identity in pool.Identities)
            {
                foreach (var relatedIdentity in identity.IdentitiesToEstablishRelationshipsWith)
                {
                    res.EstablishRelationship(identity.UniqueOrderNumber, relatedIdentity.UniqueOrderNumber);
                }

                foreach (var relatedIdentity in identity.IdentitiesToSendMessagesTo)
                {
                    res.SendMessage(identity.UniqueOrderNumber, relatedIdentity.UniqueOrderNumber);
                }
            }
        }

        return res;
    }

    /// <summary>
    /// Randomly generates a new solution. If I and maxI are provided, certain operations are preferred in the first iterations.
    /// </summary>
    /// <param name="currentSolution"></param>
    /// <param name="connectorMessageRatio"></param>
    /// <param name="i"></param>
    /// <param name="maxI"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private SolutionRepresentation GetNextState(SolutionRepresentation currentSolution, decimal connectorMessageRatio = 0.75m, uint i = 0, uint maxI = 0)
    {
        if (currentSolution.Clone() is not SolutionRepresentation solution || solution.GetType() != typeof(SolutionRepresentation))
            throw new Exception("clone failed");

        // -1 means random
        var progress = maxI > i ? Convert.ToDouble(i) / maxI : -1;

        if (progress > 0.3 || _localRandom.NextBoolean())
        {
            // Will mess with messages
            if (_localRandom.NextDouble() > 0.70)
            {
                // will remove a message
                solution.RemoveRandomMessage();
                if (PRINT_OUTPUT) Console.Write("rmv msg");
            }
            else
            {
                // will add a message
                if (_localRandom.NextDouble() > Convert.ToDouble(connectorMessageRatio))
                {
                    // app to connector
                    var appIdentity = _localRandom.GetRandomElement(_appIdentitiesDictionary);
                    var candidates = solution.GetRelationshipsAndMessageSentCountByIdentity(appIdentity.UniqueOrderNumber).ToList();
                    if (candidates.Count == 0)
                        return GetNextState(currentSolution, connectorMessageRatio);
                    var orderedCandidates = candidates.OrderByDescending(it => appIdentity.Pool.NumberOfSentMessages - it.messageCount);
                    var selectedCandidate = orderedCandidates.FirstOrDefault().relatedIdentity;
                    solution.SendMessage(appIdentity.UniqueOrderNumber, selectedCandidate);
                }
                else
                {
                    // connector to app
                    var connectorIdentity = _localRandom.GetRandomElement(_connectorIdentitiesDictionary);
                    var candidates = solution.GetRelationshipsAndMessageSentCountByIdentity(connectorIdentity.UniqueOrderNumber).ToList();
                    if (candidates.Count == 0)
                        return GetNextState(currentSolution, connectorMessageRatio);
                    var orderedCandidates = candidates.OrderByDescending(it => connectorIdentity.Pool.NumberOfSentMessages - it.messageCount);
                    var selectedCandidate = orderedCandidates.FirstOrDefault().relatedIdentity;
                    solution.SendMessage(connectorIdentity.UniqueOrderNumber, selectedCandidate);
                }

                if (PRINT_OUTPUT) Console.Write("add msg");
            }
        }
        else
        {
            // Will mess with relationships
            if (_localRandom.NextDouble() > 0.60)
            {
                // will remove a relationship
                solution.RemoveRandomRelationship();
                if (PRINT_OUTPUT) Console.Write("rmv rel");
            }
            else
            {
                // will add a relationship
                var flag = false;
                var iter = 0;
                do
                {
                    var i1 = _localRandom.GetRandomElement(_appIdentitiesDictionary);
                    if (i1.Pool.NumberOfRelationships * TOLERANCE <= solution.GetNumberOfRelatedIdentities(i1.UniqueOrderNumber)) continue;

                    // if the selected identity has capacity for further relationships
                    var i2 = _localRandom.GetRandomElement(_connectorIdentitiesDictionary);

                    solution.EstablishRelationship(i1.UniqueOrderNumber, i2.UniqueOrderNumber);
                    flag = true;
                } while (!flag && iter++ < 20);

                if (PRINT_OUTPUT) Console.Write("add rel");
            }
        }

        return solution;
    }

    private const double TOLERANCE = 1.05;

    private long CalculateScore(SolutionRepresentation solution, IDictionary<uint, GeneratorIdentity> identities)
    {
        var relationshipsTarget = Pools.ExpectedNumberOfRelationships();
        var sentMessagesTarget = Pools.ExpectedNumberOfSentMessages();

        var messagesScore = 0;
        var sentAndReceivedMessageCountByIdentityDictionary = solution.GetSentAndReceivedMessageCountByIdentityDictionary();

        foreach (var (identity, (messageSentCount, messageReceivedCount)) in sentAndReceivedMessageCountByIdentityDictionary)
        {
            var sentDiff = Convert.ToInt32(messageSentCount) - Convert.ToInt32(identities[identity].Pool.NumberOfSentMessages);
            var receivedDiff = Convert.ToInt32(messageReceivedCount) - Convert.ToInt32(identities[identity].Pool.NumberOfReceivedMessages);

            if (identities[identity].Pool.NumberOfSentMessages == 0 || sentDiff > 4)
                messagesScore -= 10 * sentDiff;
            if (identities[identity].Pool.NumberOfReceivedMessages == 0 || receivedDiff > 4)
                messagesScore -= 10 * receivedDiff;
        }

        var invalidRelationshipCount = solution.GetInvalidRelationshipCount(_identitiesDictionary);

        return
            -8 * invalidRelationshipCount - 4 * Math.Abs(relationshipsTarget - solution.GetRelationshipCount())
                                          - 4 * Math.Abs(sentMessagesTarget - solution.GetSentMessagesCount()) + messagesScore;
    }

    private void CreateIdentities()
    {
        uint globalIterator = 0;

        foreach (var poolEntry in Pools.Where(p => p.Amount > 0))
        {
            for (uint i = 0; i < poolEntry.Amount; i++)
            {
                poolEntry.Identities.Add(new GeneratorIdentity(
                        new UserCredentials("USR" + PasswordHelper.GeneratePassword(8, 8), PasswordHelper.GeneratePassword(18, 24)),
                        "ID1" + PasswordHelper.GeneratePassword(16, 16),
                        "DVC" + PasswordHelper.GeneratePassword(8, 8),
                        poolEntry,
                        i,
                        globalIterator++
                    )
                );
            }
        }
    }
}

public class SolutionRepresentation : ICloneable
{
    private readonly Stopwatch _createdAt = Stopwatch.StartNew();

    private ulong _relationshipCount;
    private ulong _messagesCount;

    /// <summary>
    /// Relationships & Messages
    /// The key represents a pair of identities by their UON.
    /// The value determines the number of messages exchanged between a & b.
    /// Reflection is an issue and must be handled carefully.
    /// This can never happen: a == b
    /// This approach ensures that messages can only be sent in the context of a relationship (a mandatory requisite).
    /// </summary>
    /// <see cref="GeneratorIdentity.UniqueOrderNumber"/>
    private Dictionary<(uint a, uint b), uint> RelationshipsAndMessages { get; } = new();

    public ulong IterationCount { get; set; }

    private readonly Random _localRandom = new();

    public SolutionRepresentation(Dictionary<(uint a, uint b), uint>? relationshipsAndMessages = null, Stopwatch? createdAt = null, (ulong relationshipCount, ulong messagesCount)? counters = null)
    {
        if (relationshipsAndMessages is not null) RelationshipsAndMessages = relationshipsAndMessages.ToDictionary(entry => entry.Key, entry => entry.Value);
        if (counters.HasValue)
        {
            _relationshipCount = counters.Value.relationshipCount;
            _messagesCount = counters.Value.messagesCount;
        }

        if (createdAt is not null) _createdAt = createdAt;
    }

    public bool EstablishRelationship(uint identity1, uint identity2)
    {
        if (RelationshipsAndMessages.ContainsKey((identity1, identity2)))
        {
            // relationship already exists
            return false;
        }

        if (identity1 == identity2)
            return false;

        RelationshipsAndMessages[(identity1, identity2)] = 0;
        RelationshipsAndMessages[(identity2, identity1)] = 0;

        _relationshipCount++;
        return true;
    }

    private TimeSpan GetTimeSinceStart() => _createdAt.Elapsed;

    /// <summary></summary>
    /// <param name="identity1"></param>
    /// <param name="identity2"></param>
    /// <returns>
    /// -1 in case of error. Otherwise, the number of messages deleted along with the relationship.
    /// </returns>
    private int RemoveRelationship(uint identity1, uint identity2)
    {
        if (_relationshipCount <= 0 || !RelationshipsAndMessages.ContainsKey((identity1, identity2)))
            return -1;

        var messageCount = RelationshipsAndMessages[(identity1, identity2)] + RelationshipsAndMessages[(identity2, identity1)];

        RelationshipsAndMessages.Remove((identity1, identity2));
        RelationshipsAndMessages.Remove((identity2, identity1));
        _messagesCount -= messageCount;
        _relationshipCount--;
        return Convert.ToInt32(messageCount);
    }

    public bool SendMessage(uint identityFrom, uint identityTo)
    {
        EstablishRelationship(identityFrom, identityTo);
        RelationshipsAndMessages[(identityFrom, identityTo)]++;
        _messagesCount++;
        return true;
    }

    private void RemoveMessage(uint identityFrom, uint identityTo)
    {
        if (!RelationshipsAndMessages.ContainsKey((identityFrom, identityTo))) return;

        RelationshipsAndMessages[(identityFrom, identityTo)]--;
        _messagesCount--;
    }

    public object Clone()
    {
        var ret = new SolutionRepresentation(RelationshipsAndMessages, counters: (relationshipCount: _relationshipCount, messagesCount: _messagesCount), createdAt: _createdAt);
        return ret;
    }

    public void RemoveRandomMessage()
    {
        if (_messagesCount <= 0) return;

        var (a, b) = _localRandom.GetRandomKey(RelationshipsAndMessages.Where(m => m.Value > 0).ToDictionary());
        RemoveMessage(a, b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// -1 in case of error. Otherwise, the number of messages deleted along with the relationship.
    /// </returns>
    public int RemoveRandomRelationship()
    {
        if (_relationshipCount <= 0) return -1;

        var (a, b) = _localRandom.GetRandomKey(RelationshipsAndMessages);
        return RemoveRelationship(a, b);
    }

    public long GetInvalidRelationshipCount(Dictionary<uint, GeneratorIdentity> identities)
    {
        var res = 0;
        foreach (var ((from, to), _) in RelationshipsAndMessages)
        {
            if (from == to)
            {
                res++;
                continue;
            }

            var i1Pool = identities[from].Pool;
            var i2Pool = identities[to].Pool;
            if (i1Pool.Type == i2Pool.Type || !i1Pool.IsApp() && !i2Pool.IsApp() || !i1Pool.IsConnector() && !i2Pool.IsConnector())
                res++;
        }

        return res;
    }

    public long GetRelationshipCount()
    {
        return Convert.ToInt64(_relationshipCount);
    }

    public long GetSentMessagesCount()
    {
        return Convert.ToInt64(_messagesCount);
    }

    public void Print(Dictionary<uint, GeneratorIdentity> identities, List<PoolEntry> pools)
    {
        Console.WriteLine(" ========= SOLUTION OUTPUT =========");
        Console.WriteLine($" =====> EXECUTION TIME: {GetTimeSinceStart()}");
        Console.WriteLine($" ====> ITERATION COUNT: {IterationCount + 1}");
        Console.WriteLine($" ==> Expected relationships: {pools.ExpectedNumberOfRelationships()}");
        Console.WriteLine($" ==> Expected sent messages: {pools.ExpectedNumberOfSentMessages()}");
        Console.WriteLine(" =====> ESTABLISHED RELATIONSHIPS");

        if (RelationshipsAndMessages.Count / 2 != GetRelationshipCount())
            Console.WriteLine("==[ERROR]=> Relationship Count mismatch.");

        if (RelationshipsAndMessages.Sum(x => x.Value) != GetSentMessagesCount())
            Console.WriteLine("==[ERROR]=> Message Count mismatch.");

        Console.WriteLine($" - Counted {RelationshipsAndMessages.Count} entries, meaning there are {RelationshipsAndMessages.Count / 2} relationships.");
        Console.WriteLine($" - Counted {RelationshipsAndMessages.Sum(x => x.Value)} messages.");
    }

    // reflections may be counted twice here
    public uint GetNumberOfRelatedIdentities(uint identity)
    {
        return (uint)RelationshipsAndMessages.Count(r =>
        {
            var ((a, b), _) = r;
            return a == identity || b == identity;
        });
    }

    public Dictionary<uint, (uint sent, uint received)> GetSentAndReceivedMessageCountByIdentityDictionary()
    {
        var res = new Dictionary<uint, (uint sent, uint received)>();

        foreach (var ((a, b), c) in RelationshipsAndMessages)
        {
            res.TryAdd(a, (0, 0));
            res.TryAdd(b, (0, 0));

            res[a] = (res[a].sent + c, res[a].received);
            res[b] = (res[b].sent, res[b].received + c);
        }

        return res;
    }

    public IEnumerable<(uint relatedIdentity, uint messageCount)> GetRelationshipsAndMessageSentCountByIdentity(uint uon)
    {
        return RelationshipsAndMessages.Where(it => it.Key.a == uon).Select(it => (it.Key.b, it.Value)).ToList();
    }

    public string GetAsCSV(Dictionary<uint, GeneratorIdentity> identitiesDictionary)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Identity1;Identity1Pool;Identity2;Identity2Pool;MessageCount");

        foreach (var ((a, b), c) in RelationshipsAndMessages)
        {
            var identity1Pool = identitiesDictionary[a].Pool.Alias;
            var identity2Pool = identitiesDictionary[b].Pool.Alias;
            stringBuilder.AppendLine($"{a};{identity1Pool};{b};{identity2Pool};{c}");
        }

        return stringBuilder.ToString();
    }

    internal uint GetIdentityCount() => RelationshipsAndMessages.Max(r => r.Key.a);
}

public static class RandomMethodExtensions
{
    extension(Random random)
    {
        public bool NextBoolean()
        {
            return random.NextDouble() <= 0.5;
        }

        public TU GetRandomElement<TU>(IList<TU> list)
        {
            var randomElementIndex = Convert.ToInt32((int)(random.NextInt64() % list.Count));
            return list[randomElementIndex];
        }

        public T GetRandomElement<T, TU>(IDictionary<TU, T> dictionary)
        {
            var randomElementIndex = Convert.ToInt32((int)(random.NextInt64() % dictionary.Count));
            return dictionary[dictionary.Keys.Skip(randomElementIndex - 1).First()];
        }

        public TK GetRandomKey<TK, TV>(IDictionary<TK, TV> dictionary)
        {
            return random.GetRandomElement(dictionary.Select(r => r.Key).ToList());
        }
    }
}
