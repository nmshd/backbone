using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.Crypto;

namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public static class Utils
{
    public static async Task EstablishRelationshipBetween(Client client1, Client client2)
    {
        var createRelationshipTemplateRequest = new CreateRelationshipTemplateRequest
        {
            Content = ConvertibleString.FromUtf8("AAA").BytesRepresentation
        };

        var relationshipTemplateResponse = await client1.RelationshipTemplates.CreateTemplate(createRelationshipTemplateRequest);
        relationshipTemplateResponse.Should().BeASuccess();

        var createRelationshipRequest = new CreateRelationshipRequest
        {
            RelationshipTemplateId = relationshipTemplateResponse.Result!.Id,
            Content = ConvertibleString.FromUtf8("AAA").BytesRepresentation
        };

        var createRelationshipResponse = await client2.Relationships.CreateRelationship(createRelationshipRequest);
        createRelationshipResponse.Should().BeASuccess();

        var completeRelationshipChangeRequest = new CompleteRelationshipChangeRequest
        {
            Content = ConvertibleString.FromUtf8("AAA").BytesRepresentation
        };
        var acceptRelationChangeResponse =
            await client1.Relationships.AcceptChange(createRelationshipResponse.Result!.Id, createRelationshipResponse.Result.Changes.First().Id, completeRelationshipChangeRequest);
        acceptRelationChangeResponse.Should().BeASuccess();
    }
}
