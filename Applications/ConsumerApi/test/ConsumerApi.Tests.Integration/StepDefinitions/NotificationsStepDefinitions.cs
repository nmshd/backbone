using Backbone.ConsumerApi.Sdk.Endpoints.Notifications.Types;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class NotificationsStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public NotificationsStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region When

    [When($"^{RegexFor.SINGLE_THING} sends a POST request to the /Notifications endpoint with {RegexFor.SINGLE_THING} as recipient$")]
    public async Task WhenIdentitySendsAPostRequestToTheTokensEndpoint(string senderName, string recipientName)
    {
        var senderClient = _clientPool.FirstForIdentityName(senderName);
        var recipientAddress = _clientPool.FirstForIdentityName(recipientName).IdentityData!.Address;

        _responseContext.WhenResponse = await senderClient.Notifications.SendNotification(new SendNotificationRequest
        {
            Code = "TestCode",
            Recipients = [recipientAddress]
        });
    }

    #endregion

    [When($"^{RegexFor.SINGLE_THING} sends a POST request to the /Notifications endpoint with {RegexFor.SINGLE_THING} as recipient and a non existing code$")]
    public async Task WhenISendsAPOSTRequestToTheNotificationsEndpointWithIAsRecipientAndANonExistingCode(string senderName, string recipientName)
    {
        var senderClient = _clientPool.FirstForIdentityName(senderName);
        var recipientAddress = _clientPool.FirstForIdentityName(recipientName).IdentityData!.Address;

        _responseContext.WhenResponse = await senderClient.Notifications.SendNotification(new SendNotificationRequest
        {
            Code = "NonExistingCode",
            Recipients = [recipientAddress]
        });
    }
}
