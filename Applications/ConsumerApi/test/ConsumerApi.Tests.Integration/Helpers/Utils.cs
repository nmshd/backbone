using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.StepDefinitions;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Backbone.Tooling.Extensions;
using Backbone.UnitTestTools.Data;
using Newtonsoft.Json;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

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

    public static async Task<Relationship> EstablishRelationshipBetween(Client client1, Client client2)
    {
        var createRelationshipTemplateRequest = new CreateRelationshipTemplateRequest
        {
            Content = "AAA".GetBytes()
        };

        var relationshipTemplateResponse = await client1.RelationshipTemplates.CreateTemplate(createRelationshipTemplateRequest);
        relationshipTemplateResponse.Should().BeASuccess();

        var createRelationshipRequest = new CreateRelationshipRequest
        {
            RelationshipTemplateId = relationshipTemplateResponse.Result!.Id,
            Content = "AAA".GetBytes()
        };

        var createRelationshipResponse = await client2.Relationships.CreateRelationship(createRelationshipRequest);
        createRelationshipResponse.Should().BeASuccess();

        var acceptRelationshipRequest = new AcceptRelationshipRequest
        {
            CreationResponseContent = "AAA".GetBytes()
        };

        var acceptRelationshipResponse = await client1.Relationships.AcceptRelationship(createRelationshipResponse.Result!.Id, acceptRelationshipRequest);
        acceptRelationshipResponse.Should().BeASuccess();

        var getRelationshipResponse = await client1.Relationships.GetRelationship(createRelationshipResponse.Result.Id);
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
