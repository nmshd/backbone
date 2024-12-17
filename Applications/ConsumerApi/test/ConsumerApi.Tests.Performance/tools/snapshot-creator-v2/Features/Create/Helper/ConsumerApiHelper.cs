using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Crypto;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public class ConsumerApiHelper : IConsumerApiHelper
{
    public Task<Client> CreateForNewIdentity(CreateIdentities.Command request) =>
        Client.CreateForNewIdentity(request.BaseUrlAddress, request.ClientCredentials, PasswordHelper.GeneratePassword(18, 24));

    public Client CreateForExistingIdentity(string baseUrl, ClientCredentials clientCredentials, UserCredentials userCredentials, IdentityData? identityData = null) =>
        Client.CreateForExistingIdentity(baseUrl, clientCredentials, userCredentials, identityData);

    public async Task<string> OnBoardNewDevice(DomainIdentity identity, Client sdkClient)
    {
        var newDevice = await sdkClient.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));

        return newDevice.DeviceData is null
            ? throw new InvalidOperationException(
                $"The SDK could not be used to create a new database Device for config {identity.IdentityAddress}/{identity.ConfigurationIdentityAddress}/{identity.PoolAlias} {IDENTITY_LOG_SUFFIX}")
            : newDevice.DeviceData.DeviceId;
    }

    public Task<ApiResponse<Challenge>> CreateChallenge(Client sdkClient) => sdkClient.Challenges.CreateChallenge();


    public Task<ApiResponse<StartSyncRunResponse>> StartSyncRun(Client sdk) => sdk.SyncRuns.StartSyncRun(
        new StartSyncRunRequest
        {
            Type = SyncRunType.DatawalletVersionUpgrade,
            Duration = 100
        },
        1);

    public Task<ApiResponse<FinalizeDatawalletVersionUpgradeResponse>> FinalizeDatawalletVersionUpgrade(DomainIdentity identity,
        Client sdk,
        ApiResponse<StartSyncRunResponse> startDatawalletVersionUpgradeResponse) => sdk.SyncRuns.FinalizeDatawalletVersionUpgrade(startDatawalletVersionUpgradeResponse.Result!.SyncRun.Id,
        new FinalizeDatawalletVersionUpgradeRequest
        {
            DatawalletModifications = PreGenerateDatawalletModifications(identity.NumberOfDatawalletModifications),
            NewDatawalletVersion = 3
        });


    public async Task<ApiResponse<CreateRelationshipTemplateResponse>> CreateRelationshipTemplate(Client sdkClient)
    {
        return await sdkClient.RelationshipTemplates.CreateTemplate(
            new CreateRelationshipTemplateRequest
            {
                Content = [],
                ExpiresAt = DateTime.Now.EndOfYear(),
                MaxNumberOfAllocations = 1000
            });
    }

    public Task<ApiResponse<RelationshipMetadata>> CreateRelationship(Client appIdentitySdkClient, RelationshipTemplateBag nextRelationshipTemplate) =>
        appIdentitySdkClient.Relationships.CreateRelationship(
            new CreateRelationshipRequest
            {
                RelationshipTemplateId = nextRelationshipTemplate.Template.Id,
                Content = []
            });

    public Task<ApiResponse<RelationshipMetadata>> AcceptRelationship(Client connectorIdentitySdkClient, ApiResponse<RelationshipMetadata> createRelationshipResponse) =>
        connectorIdentitySdkClient.Relationships.AcceptRelationship(
            createRelationshipResponse.Result!.Id,
            new AcceptRelationshipRequest());


    public Task<ApiResponse<SendMessageResponse>> SendMessage(DomainIdentity recipientIdentity, Client senderIdentitySdkClient) =>
        senderIdentitySdkClient.Messages.SendMessage(new SendMessageRequest
        {
            Recipients =
            [
                new SendMessageRequestRecipientInformation
                {
                    Address = recipientIdentity.IdentityAddress!,
                    EncryptedKey = ConvertibleString.FromUtf8(new string('A', 152)).BytesRepresentation
                }
            ],
            Attachments = [],
            Body = ConvertibleString.FromUtf8("Message body").BytesRepresentation
        });

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

    private static T GetRandomElement<T, TU>(Random random, Dictionary<TU, T> dictionary) where TU : notnull
    {
        var randomElementIndex = Convert.ToInt32(random.NextInt64() % dictionary.Count);
        return dictionary[dictionary.Keys.Skip(randomElementIndex - 1).First()];
    }
}
