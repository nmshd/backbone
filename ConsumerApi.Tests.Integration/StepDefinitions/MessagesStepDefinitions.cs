using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.Crypto;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class MessagesStepDefinitions
{   
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public MessagesStepDefinitions(IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];

    [When("(.+) sends a POST request to the Messages endpoint with (.+) as recipient")]
    public async Task WhenAPostRequestIsSentToTheMessagesEndpoint(string identityName1, string identityName2)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Attachments = [],
            Body = ConvertibleString.FromUtf8("Some Message").BytesRepresentation,
            Recipients =
            [
                new SendMessageRequestRecipientInformation
                {
                    Address = Identity(identityName2).IdentityData!.Address,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        };

        _responseContext.WhenResponse = _responseContext.SendMessageResponse = await Identity(identityName1).Messages.SendMessage(sendMessageRequest);
    }
}

public class PeersToBeDeletedErrorData
{
    public required List<string> PeersToBeDeleted { get; set; }
}
