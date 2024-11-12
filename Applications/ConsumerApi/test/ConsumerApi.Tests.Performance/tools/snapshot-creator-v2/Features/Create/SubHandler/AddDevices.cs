using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record AddDevices
{
    public record Command(List<DomainIdentity> Identities, string BaseUrlAddress, ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithDevices = request.Identities.Where(i => i.NumberOfDevices > 0);

            foreach (var identity in identitiesWithDevices)
            {
                var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials);

                for (var i = 0; i < identity.NumberOfDevices; i++)
                {
                    var newDevice = await sdkClient.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));

                    if (newDevice.DeviceData is null)
                        throw new Exception("The SDK could not be used to create a new database Device or the DeviceData is null.");


                    identity.AddDevice(newDevice.DeviceData.DeviceId);
                }
            }

            return Unit.Value;
        }
    }
}
