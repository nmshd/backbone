using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public static class Utils
{
    public static SignedChallenge CreateSignedChallenge(Client identity, Challenge challenge)
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
        var identityKeyPair = identity.IdentityData!.KeyPair;

        var serializedChallenge = JsonConvert.SerializeObject(challenge);
        var challengeSignature = signatureHelper.CreateSignature(identityKeyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));

        return new SignedChallenge(serializedChallenge, challengeSignature);
    }

    public static async Task<Relationship> CreatePendingRelationshipBetween(Client templator, Client requestor)
    {
        var createRelationshipTemplateRequest = new CreateRelationshipTemplateRequest
        {
            Content = TestData.SOME_BYTES
        };

        var relationshipTemplateResponse = await templator.RelationshipTemplates.CreateTemplate(createRelationshipTemplateRequest);
        relationshipTemplateResponse.ShouldBeASuccess();

        return await CreatePendingRelationshipUsingTemplate(requestor, relationshipTemplateResponse.Result!.Id);
    }

    public static async Task<Relationship> CreatePendingRelationshipUsingTemplate(Client requestor, string templateId)
    {
        var createRelationshipRequest = new CreateRelationshipRequest
        {
            RelationshipTemplateId = templateId,
            Content = TestData.SOME_BYTES
        };

        var createRelationshipResponse = await requestor.Relationships.CreateRelationship(createRelationshipRequest);
        createRelationshipResponse.ShouldBeASuccess();

        var getRelationshipResponse = await requestor.Relationships.GetRelationship(createRelationshipResponse.Result!.Id);
        getRelationshipResponse.ShouldBeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<Relationship> EstablishRelationshipBetween(Client client1, Client client2)
    {
        var pendingRelationship = await CreatePendingRelationshipBetween(client1, client2);

        var acceptRelationshipRequest = new AcceptRelationshipRequest
        {
            CreationResponseContent = TestData.SOME_BYTES
        };

        var acceptRelationshipResponse = await client1.Relationships.AcceptRelationship(pendingRelationship.Id, acceptRelationshipRequest);
        acceptRelationshipResponse.ShouldBeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(pendingRelationship.Id);
        getRelationshipResponse.ShouldBeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<Relationship> EstablishRelationshipBetween(Client client1, Client client2, string templateId)
    {
        var pendingRelationship = await CreatePendingRelationshipUsingTemplate(client2, templateId);

        var acceptRelationshipRequest = new AcceptRelationshipRequest
        {
            CreationResponseContent = TestData.SOME_BYTES
        };

        var acceptRelationshipResponse = await client1.Relationships.AcceptRelationship(pendingRelationship.Id, acceptRelationshipRequest);
        acceptRelationshipResponse.ShouldBeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(pendingRelationship.Id);
        getRelationshipResponse.ShouldBeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<Relationship> CreateRejectedRelationshipBetween(Client client1, Client client2)
    {
        var relationshipMetadata = await CreatePendingRelationshipBetween(client1, client2);

        var rejectRelationshipRequest = new RejectRelationshipRequest
        {
            CreationResponseContent = TestData.SOME_BYTES
        };

        var rejectRelationshipResponse = await client1.Relationships.RejectRelationship(relationshipMetadata.Id, rejectRelationshipRequest);
        rejectRelationshipResponse.ShouldBeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(relationshipMetadata.Id);
        getRelationshipResponse.ShouldBeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<Relationship> CreateTerminatedRelationshipBetween(Client client1, Client client2)
    {
        var relationshipMetadata = await EstablishRelationshipBetween(client1, client2);

        var terminateRelationshipResponse = await client1.Relationships.TerminateRelationship(relationshipMetadata.Id);
        terminateRelationshipResponse.ShouldBeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(relationshipMetadata.Id);
        getRelationshipResponse.ShouldBeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<Relationship> CreateTerminatedRelationshipWithReactivationRequestBetween(Client client1, Client client2)
    {
        var relationshipMetadata = await CreateTerminatedRelationshipBetween(client1, client2);

        var reactivateRelationshipResponse = await client2.Relationships.ReactivateRelationship(relationshipMetadata.Id);
        reactivateRelationshipResponse.ShouldBeASuccess();

        var getRelationshipResponse = await client2.Relationships.GetRelationship(relationshipMetadata.Id);
        getRelationshipResponse.ShouldBeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<CreateFileResponse> CreateFile(Client client)
    {
        var createFileRequest = new CreateFileRequest
        {
            Content = new MemoryStream("content"u8.ToArray()),
            Owner = client.IdentityData!.Address,
            OwnerSignature = TestData.SOME_BASE64_STRING,
            CipherHash = TestData.SOME_BASE64_STRING,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            EncryptedProperties = TestData.SOME_BASE64_STRING
        };

        var createFileResponse = await client.Files.UploadFile(createFileRequest);

        createFileResponse.ShouldBeASuccess();
        return createFileResponse.Result!;
    }

    public static async Task<Message> SendMessage(Client sender, params Client[] recipients)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Body = [0],
            Recipients = recipients.Select(r => new SendMessageRequestRecipientInformation
            {
                Address = r.IdentityData!.Address,
                EncryptedKey = CreateRandomBytes(30)
            }).ToList(),
            Attachments = []
        };
        var sendMessageResponse = await sender.Messages.SendMessage(sendMessageRequest);

        sendMessageResponse.ShouldBeASuccess();

        var getMessageResponse = await sender.Messages.GetMessage(sendMessageResponse.Result!.Id);

        getMessageResponse.ShouldBeASuccess();

        return getMessageResponse.Result!;
    }

    public static List<string> SplitNames(string names)
    {
        return names.Split([", ", " and "], StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
