using System.Diagnostics;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public class DeviceFactory(ILogger<DeviceFactory> logger, IConsumerApiHelper consumerApiHelper) : IDeviceFactory
{
    internal int NumberOfCreatedDevices;
    public int TotalNumberOfDevices { get; set; }
    private readonly Lock _lockObj = new();
    internal readonly SemaphoreSlim _semaphoreSlim = new(Environment.ProcessorCount);

    public async Task Create(CreateDevices.Command request, DomainIdentity identity)
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            Stopwatch stopwatch = new();

            stopwatch.Start();
            var deviceIds = await CreateDevices(request, identity);
            stopwatch.Stop();

            using (_lockObj.EnterScope())
            {
                NumberOfCreatedDevices += deviceIds.Count;
            }

            logger.LogDebug(
                "Created {CreatedDevices}/{TotalNumberOfDevices} devices.  Semaphore.Count: {SemaphoreCount} - Devices {DeviceIds} of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                NumberOfCreatedDevices,
                TotalNumberOfDevices,
                _semaphoreSlim.CurrentCount,
                string.Join(',', deviceIds),
                identity.IdentityAddress,
                identity.ConfigurationIdentityAddress,
                identity.PoolAlias,
                stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    internal async Task<List<string>> CreateDevices(CreateDevices.Command request, DomainIdentity identity)
    {
        List<string> deviceIds = [];

        var sdkClient = consumerApiHelper.CreateForExistingIdentity(request, identity);

        // Note: The reason for starting the loop at 1 is that the first device is already created in the IdentityFactory
        for (var i = 1; i < identity.NumberOfDevices; i++)
        {
            var newDeviceId = await consumerApiHelper.OnBoardNewDevice(identity, sdkClient);

            deviceIds.Add(newDeviceId);
        }

        identity.AddDevices(deviceIds);
        return deviceIds;
    }
}
