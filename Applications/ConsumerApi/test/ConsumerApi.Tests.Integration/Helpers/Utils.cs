using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Backbone.UnitTestTools.Data;
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

    public static async Task<Relationship> CreatePendingRelationshipBetween(Client client1, Client client2)
    {
        var createRelationshipTemplateRequest = new CreateRelationshipTemplateRequest
        {
            Content = TestData.SOME_BYTES
        };

        var relationshipTemplateResponse = await client1.RelationshipTemplates.CreateTemplate(createRelationshipTemplateRequest);
        relationshipTemplateResponse.Should().BeASuccess();

        var createRelationshipRequest = new CreateRelationshipRequest
        {
            RelationshipTemplateId = relationshipTemplateResponse.Result!.Id,
            Content = TestData.SOME_BYTES
        };

        var createRelationshipResponse = await client2.Relationships.CreateRelationship(createRelationshipRequest);
        createRelationshipResponse.Should().BeASuccess();

        var getRelationshipResponse = await client2.Relationships.GetRelationship(createRelationshipResponse.Result!.Id);
        getRelationshipResponse.Should().BeASuccess();

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
        acceptRelationshipResponse.Should().BeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(pendingRelationship.Id);
        getRelationshipResponse.Should().BeASuccess();

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
        rejectRelationshipResponse.Should().BeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(relationshipMetadata.Id);
        getRelationshipResponse.Should().BeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<Relationship> CreateTerminatedRelationshipBetween(Client client1, Client client2)
    {
        var relationshipMetadata = await EstablishRelationshipBetween(client1, client2);

        var terminateRelationshipResponse = await client1.Relationships.TerminateRelationship(relationshipMetadata.Id);
        terminateRelationshipResponse.Should().BeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(relationshipMetadata.Id);
        getRelationshipResponse.Should().BeASuccess();

        return getRelationshipResponse.Result!;
    }

    public static async Task<Message> SendMessage(Client sender, params Client[] recipients)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Body = [0],
            Recipients = recipients.Select(r => new SendMessageRequestRecipientInformation
            {
                Address = r.IdentityData!.Address,
                EncryptedKey = TestDataGenerator.CreateRandomBytes(30)
            }).ToList(),
            Attachments = []
        };
        var sendMessageResponse = await sender.Messages.SendMessage(sendMessageRequest);

        sendMessageResponse.Should().BeASuccess();

        var getMessageResponse = await sender.Messages.GetMessage(sendMessageResponse.Result!.Id);

        getMessageResponse.Should().BeASuccess();

        return getMessageResponse.Result!;
    }

    public static List<string> SplitNames(string names)
    {
        return names.Split([", ", " and "], StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
