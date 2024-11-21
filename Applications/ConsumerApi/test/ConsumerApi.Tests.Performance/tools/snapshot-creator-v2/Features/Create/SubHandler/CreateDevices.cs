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
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var totalNumberOfDevices = request.Identities.Sum(i => i.NumberOfDevices);
            var numberOfCreatedDevices = 0;

            foreach (var identity in request.Identities)
            {
                Stopwatch stopwatch = new();

                stopwatch.Start();
                var deviceIds = await CreateDevices(request, identity);
                stopwatch.Stop();

                numberOfCreatedDevices += deviceIds.Count;
                Logger.LogDebug("Created {CreatedDevices}/{TotalNumberOfDevices} devices. Devices {DeviceIds} of Identity {Address}/{ConfigurationAddress}/{Pool} created in {ElapsedMilliseconds} ms",
                    numberOfCreatedDevices,
                    totalNumberOfDevices,
                    string.Join(',', deviceIds),
                    identity.IdentityAddress,
                    identity.ConfigurationIdentityAddress,
                    identity.PoolAlias,
                    stopwatch.ElapsedMilliseconds);
            }

            return Unit.Value;
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
