using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.StepDefinitions;
using Backbone.Tooling.Extensions;
using Backbone.UnitTestTools.Data;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public static class Utils
{
    public static void CreateUnauthenticated(IdentitiesContext identitiesContext, HttpClient httpClient, ClientCredentials clientCredentials)
    {
    }

    public static async Task CreateAuthenticated(IdentitiesContext identitiesContext, HttpClient httpClient, ClientCredentials clientCredentials, string identityName)
    {
        identitiesContext.Identities.Add(identityName, await Client.CreateForNewIdentity(httpClient, clientCredentials, DEVICE_PASSWORD));
    }

    public static async Task<Relationship> EstablishRelationshipBetween(Client client1, Client client2)
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
}
