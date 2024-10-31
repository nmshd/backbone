using System.Net.Sockets;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.Tooling;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;

public record AddDevices
{
    public record Command(List<DomainIdentity> Identities, string BaseUrl, ClientCredentials ClientCredentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CommandHandler> Logger) : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Adding devices ...");

            var identitiesWithDevices = request.Identities.Where(i => i.NumberOfDevices > 0);

            foreach (var identity in identitiesWithDevices)
            {
                var sdkClient = Client.CreateForExistingIdentity(request.BaseUrl, request.ClientCredentials, identity.UserCredentials);

                for (var i = 0; i < identity.NumberOfDevices; i++)
                {
                    var newDevice = await sdkClient.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));

                    if (newDevice.DeviceData is null)
                        throw new Exception("The SDK could not be used to create a new database Device or the DeviceData is null.");


                    identity.AddDevice(newDevice.DeviceData.DeviceId);
                }
            }

            return request.Identities;
        }
    }
}
