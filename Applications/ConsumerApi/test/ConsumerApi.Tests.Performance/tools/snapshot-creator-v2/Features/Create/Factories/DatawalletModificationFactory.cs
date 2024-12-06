using System.Diagnostics;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public class DatawalletModificationFactory(ILogger<DatawalletModificationFactory> logger, IConsumerApiHelper consumerApiHelper) : IDatawalletModificationFactory
{
    internal int NumberOfCreatedDatawalletModifications;
    public int TotalDatawalletModifications { get; set; }
    private readonly Lock _lockObj = new();
    internal readonly SemaphoreSlim SemaphoreSlim = new(Environment.ProcessorCount);

    public async Task Create(CreateDatawalletModifications.Command request, DomainIdentity identity)
    {
        await SemaphoreSlim.WaitAsync();
        try
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

            using (_lockObj.EnterScope())
            {
                NumberOfCreatedDatawalletModifications += finalizeDatawalletVersionUpgradeResponse.Result.DatawalletModifications.Count;
            }

            logger.LogDebug(
                "Created {CreatedDatawalletModifications}/{TotalDatawalletModifications} datawallet modifications.  Semaphore.Count: {SemaphoreCount} - Datawallet modifications of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                NumberOfCreatedDatawalletModifications,
                TotalDatawalletModifications,
                SemaphoreSlim.CurrentCount,
                identity.IdentityAddress,
                identity.ConfigurationIdentityAddress,
                identity.PoolAlias,
                stopwatch.ElapsedMilliseconds);

            identity.SetDatawalletModifications(finalizeDatawalletVersionUpgradeResponse.Result.DatawalletModifications);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    internal async Task<ApiResponse<FinalizeDatawalletVersionUpgradeResponse>> CreateDatawalletModifications(CreateDatawalletModifications.Command request, DomainIdentity identity)
    {
        var sdk = consumerApiHelper.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials);

        var startDatawalletVersionUpgradeResponse = await consumerApiHelper.StartSyncRun(sdk);

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

        var finalizeDatawalletVersionUpgradeResponse = await consumerApiHelper.FinalizeDatawalletVersionUpgrade(identity, sdk, startDatawalletVersionUpgradeResponse);

        return finalizeDatawalletVersionUpgradeResponse.IsError
            ? throw new InvalidOperationException(BuildErrorDetails($"Failed to finalize the DataWallet Sync-Run. {nameof(finalizeDatawalletVersionUpgradeResponse)}.IsError.",
                identity,
                finalizeDatawalletVersionUpgradeResponse))
            : finalizeDatawalletVersionUpgradeResponse;
    }
}
