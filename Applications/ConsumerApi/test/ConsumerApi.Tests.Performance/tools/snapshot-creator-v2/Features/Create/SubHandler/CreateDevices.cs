using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateDevices
{
    public record Command(List<DomainIdentity> Identities, string BaseUrlAddress, ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CreateDevices> Logger) : IRequestHandler<Command, Unit>
    {
        private int _numberOfCreatedDevices;
        private int _totalNumberOfDevices;
        private readonly Lock _lockObj = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(Environment.ProcessorCount);

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            _totalNumberOfDevices = request.Identities.Sum(i => i.NumberOfDevices);
            _numberOfCreatedDevices = 0;

            var tasks = request.Identities
                .Select(identity => ExecuteCreateDevices(request, identity))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        private async Task ExecuteCreateDevices(Command request, DomainIdentity identity)
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
                    _numberOfCreatedDevices += deviceIds.Count;
                }

                Logger.LogDebug(
                    "Created {CreatedDevices}/{TotalNumberOfDevices} devices.  Semaphore.Count: {SemaphoreCount} - Devices {DeviceIds} of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                    _numberOfCreatedDevices,
                    _totalNumberOfDevices,
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

        private static async Task<List<string>> CreateDevices(Command request, DomainIdentity identity)
        {
            List<string> deviceIds = [];

            var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials, identity.IdentityData);

            if (identity.DeviceIds.Count == 1)
            {
                // Note: One Device gets already added in the Identity creation handler
                deviceIds.Add(identity.DeviceIds[0]);
            }

            for (var i = 1; i < identity.NumberOfDevices; i++)
            {
                var newDevice = await sdkClient.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));

                if (newDevice.DeviceData is null)
                {
                    throw new InvalidOperationException(
                        $"The SDK could not be used to create a new database Device for config {identity.IdentityAddress}/{identity.ConfigurationIdentityAddress}/{identity.PoolAlias} {IDENTITY_LOG_SUFFIX}");
                }

                deviceIds.Add(newDevice.DeviceData.DeviceId);
            }

            identity.AddDevices(deviceIds);

            return deviceIds;
        }
    }
}
