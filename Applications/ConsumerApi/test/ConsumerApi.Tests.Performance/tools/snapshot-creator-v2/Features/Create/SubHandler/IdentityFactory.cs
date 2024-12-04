using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public class IdentityFactory(ILogger<IdentityFactory> logger) : IIdentityFactory
{
    private int _numberOfCreatedIdentities;
    public int TotalIdentities { get; set; }


    private readonly Lock _lockObj = new();
    private readonly SemaphoreSlim _semaphoreSlim = new(Environment.ProcessorCount);

    public async Task<DomainIdentity> Create(CreateIdentities.Command request, IdentityConfiguration identityConfiguration)
    {
        DomainIdentity createdIdentity;
        await _semaphoreSlim.WaitAsync();
        try
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            createdIdentity = await InnerCreate(request, identityConfiguration);
            stopwatch.Stop();

            using (_lockObj.EnterScope())
            {
                _numberOfCreatedIdentities++;
            }

            logger.LogDebug(
                "Created {CreatedIdentities}/{TotalIdentities} identities. Semaphore.Count: {SemaphoreCount} - Identity {Address}/{ConfigurationAddress}/{Pool} added in {ElapsedMilliseconds} ms",
                _numberOfCreatedIdentities,
                TotalIdentities,
                _semaphoreSlim.CurrentCount,
                createdIdentity.IdentityAddress,
                createdIdentity.ConfigurationIdentityAddress,
                createdIdentity.PoolAlias,
                stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        return createdIdentity;
    }

    private static async Task<DomainIdentity> InnerCreate(CreateIdentities.Command request, IdentityConfiguration identityConfiguration)
    {
        var sdkClient = await Client.CreateForNewIdentity(request.BaseUrlAddress, request.ClientCredentials, PasswordHelper.GeneratePassword(18, 24));

        if (sdkClient.DeviceData is null)
            throw new InvalidOperationException(
                $"The sdkClient.DeviceData is null. Could not be used to create a new database Identity for config {identityConfiguration.Address}/{identityConfiguration.PoolAlias} [IdentityAddress/Pool]");

        var createdIdentity = new DomainIdentity(
            sdkClient.DeviceData.UserCredentials,
            sdkClient.IdentityData,
            identityConfiguration.Address,
            identityConfiguration.NumberOfDevices,
            identityConfiguration.NumberOfRelationshipTemplates,
            identityConfiguration.IdentityPoolType,
            identityConfiguration.NumberOfChallenges,
            identityConfiguration.PoolAlias,
            identityConfiguration.NumberOfDatawalletModifications,
            identityConfiguration.NumberOfSentMessages);

        if (string.IsNullOrWhiteSpace(sdkClient.DeviceData.DeviceId))
        {
            throw new InvalidOperationException(
                $"The sdkClient.DeviceData.DeviceId is null or empty. Could not be used to create a new database Device for config {identityConfiguration.Address}/{identityConfiguration.PoolAlias} [IdentityAddress/Pool]");
        }

        createdIdentity.AddDevice(sdkClient.DeviceData.DeviceId);
        return createdIdentity;
    }
}
