using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record CreateDatawalletModifications
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseAddress,
        ClientCredentials ClientCredentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator
    public record CommandHandler : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithDatawalletModifications = request.Identities
                .Where(i => i.NumberOfDatawalletModifications > 0)
                .ToList();

            foreach (var identity in identitiesWithDatawalletModifications)
            {
                var sdk = Client.CreateForExistingIdentity(request.BaseAddress, request.ClientCredentials, identity.UserCredentials);

                var startDatawalletVersionUpgradeResponse = await sdk.SyncRuns.StartSyncRun(
                    new StartSyncRunRequest
                    {
                        Type = SyncRunType.DatawalletVersionUpgrade,
                        Duration = 100
                    },
                    1);

                if (startDatawalletVersionUpgradeResponse.Result is null) continue;

                var finalizeDatawalletVersionUpgradeResponse = await sdk.SyncRuns.FinalizeDatawalletVersionUpgrade(
                    startDatawalletVersionUpgradeResponse.Result.SyncRun.Id,
                    new FinalizeDatawalletVersionUpgradeRequest
                    {
                        DatawalletModifications = PreGenerateDatawalletModifications(identity.NumberOfDatawalletModifications),
                        NewDatawalletVersion = 3
                    });


                if (finalizeDatawalletVersionUpgradeResponse.Result is null) continue;

                identity.SetDatawalletModifications(finalizeDatawalletVersionUpgradeResponse.Result.DatawalletModifications);
            }

            return request.Identities;
        }

        private List<PushDatawalletModificationsRequestItem> PreGenerateDatawalletModifications(int datawalletModifications)
        {
            List<PushDatawalletModificationsRequestItem> result = [];
            var objectIterator = 1;

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
    }
}
