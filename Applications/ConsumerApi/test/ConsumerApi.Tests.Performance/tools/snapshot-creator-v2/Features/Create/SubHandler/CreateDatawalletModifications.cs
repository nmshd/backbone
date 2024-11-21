using System.Diagnostics;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateDatawalletModifications
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator
    public record CommandHandler(ILogger<CreateDatawalletModifications> Logger) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithDatawalletModifications = request.Identities
                .Where(i => i.NumberOfDatawalletModifications > 0)
                .ToList();

            var totalDatawalletModifications = identitiesWithDatawalletModifications.Sum(i => i.NumberOfDatawalletModifications);
            var numberOfCreatedDatawalletModifications = 0;

            foreach (var identity in identitiesWithDatawalletModifications)
            {
                Stopwatch stopwatch = new();

                stopwatch.Start();
                var finalizeDatawalletVersionUpgradeResponse = await CreateDatawalletModifications(request, identity);
                stopwatch.Stop();

                if (finalizeDatawalletVersionUpgradeResponse.Result is null)
                {
                    throw new InvalidOperationException(BuildErrorDetails($"Failed to finalize the DataWallet Sync-Run. {nameof(finalizeDatawalletVersionUpgradeResponse)}.Result is null.",
                        identity,
                        finalizeDatawalletVersionUpgradeResponse));
                }

                numberOfCreatedDatawalletModifications += finalizeDatawalletVersionUpgradeResponse.Result.DatawalletModifications.Count;

                Logger.LogDebug(
                    "Created {CreatedDatawalletModifications}/{TotalDatawalletModifications} datawallet modifications. Datawallet modifications of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                    numberOfCreatedDatawalletModifications,
                    totalDatawalletModifications,
                    identity.IdentityAddress,
                    identity.ConfigurationIdentityAddress,
                    identity.PoolAlias,
                    stopwatch.ElapsedMilliseconds);

                identity.SetDatawalletModifications(finalizeDatawalletVersionUpgradeResponse.Result.DatawalletModifications);
            }

            return Unit.Value;
        }

        private static async Task<ApiResponse<FinalizeDatawalletVersionUpgradeResponse>> CreateDatawalletModifications(Command request, DomainIdentity identity)
        {
            var sdk = Client.CreateForExistingIdentity(request.BaseAddress, request.ClientCredentials, identity.UserCredentials);

            var startDatawalletVersionUpgradeResponse = await sdk.SyncRuns.StartSyncRun(
                new StartSyncRunRequest
                {
                    Type = SyncRunType.DatawalletVersionUpgrade,
                    Duration = 100
                },
                1);

            if (startDatawalletVersionUpgradeResponse.IsError)
            {
                throw new InvalidOperationException(BuildErrorDetails($"Failed to start the DataWallet Sync-Run. {nameof(startDatawalletVersionUpgradeResponse)}.IsError.",
                    identity,
                    startDatawalletVersionUpgradeResponse));
            }

            if (startDatawalletVersionUpgradeResponse.Result is null)
            {
                throw new InvalidOperationException(BuildErrorDetails($"Failed to start the DataWallet Sync-Run. {nameof(startDatawalletVersionUpgradeResponse)}.Result is null.",
                    identity,
                    startDatawalletVersionUpgradeResponse));
            }

            var finalizeDatawalletVersionUpgradeResponse = await sdk.SyncRuns.FinalizeDatawalletVersionUpgrade(
                startDatawalletVersionUpgradeResponse.Result.SyncRun.Id,
                new FinalizeDatawalletVersionUpgradeRequest
                {
                    DatawalletModifications = PreGenerateDatawalletModifications(identity.NumberOfDatawalletModifications),
                    NewDatawalletVersion = 3
                });

            return finalizeDatawalletVersionUpgradeResponse.IsError
                ? throw new InvalidOperationException(BuildErrorDetails($"Failed to finalize the DataWallet Sync-Run. {nameof(finalizeDatawalletVersionUpgradeResponse)}.IsError.",
                    identity,
                    finalizeDatawalletVersionUpgradeResponse))
                : finalizeDatawalletVersionUpgradeResponse;
        }

        private static List<PushDatawalletModificationsRequestItem> PreGenerateDatawalletModifications(int datawalletModifications)
        {
            var result = new List<PushDatawalletModificationsRequestItem>();
            var objectIterator = 1;

            if (datawalletModifications < 10)
            {
                for (uint i = 0; i < datawalletModifications; i++)
                {
                    result.Add(new PushDatawalletModificationsRequestItem
                    {
                        Collection = "Performance-Tests",
                        DatawalletVersion = 2,
                        Type = "Create",
                        ObjectIdentifier = "OBJ" + objectIterator++.ToString("D12"),
                        PayloadCategory = "Userdata"
                    });
                }

                return result;
            }

            var idsAndOperationsDictionary = new Dictionary<string, List<string>>();
            var random = new Random();

            for (var i = 0; i < datawalletModifications; i++)
            {
                if (i < datawalletModifications * 3 / 10)
                {
                    // create
                    idsAndOperationsDictionary.Add("OBJ" + objectIterator++.ToString("D12"), ["Create"]);
                }
                else if (i < datawalletModifications * 9 / 10)
                {
                    // update
                    GetRandomElement(random, idsAndOperationsDictionary).Add("Update");
                }
                else
                {
                    // delete
                    var selectedKey = idsAndOperationsDictionary.Where(p => !p.Value.Contains("Delete")).Select(p => p.Key).First();
                    idsAndOperationsDictionary[selectedKey].Add("Delete");
                }
            }

            foreach (var (id, operations) in idsAndOperationsDictionary)
            {
                result.AddRange(operations.Select(operation => new PushDatawalletModificationsRequestItem
                {
                    Collection = "Requests",
                    DatawalletVersion = 1,
                    ObjectIdentifier = id,
                    Type = operation,
                    PayloadCategory = "Metadata"
                }));
            }

            return result;
        }

        private static T GetRandomElement<T, TU>(Random random, IDictionary<TU, T> dictionary)
        {
            var randomElementIndex = Convert.ToInt32(random.NextInt64() % dictionary.Count);
            return dictionary[dictionary.Keys.Skip(randomElementIndex - 1).First()];
        }
    }
}
