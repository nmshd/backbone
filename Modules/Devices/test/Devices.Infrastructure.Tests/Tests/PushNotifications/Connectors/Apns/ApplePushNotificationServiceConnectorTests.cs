using System.Net;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.Connectors.Apns;

public class ApplePushNotificationServiceConnectorTests : AbstractTestsBase
{
    private const string APP_ID = "some-app-id";

    [Fact]
    public async Task Notification_is_sent_successfully()
    {
        // Arrange
        var client = HttpClientMock.Create();
        var connector = CreateConnector(client);

        // Act
        var recipient = IdentityAddress.Parse("did:e:prod.enmeshed.eu:dids:b9d25bd0a2bbd3aa48437c");
        var registrations = new List<PnsRegistration>
        {
            new(recipient, DeviceId.New(), PnsHandle.Parse(PushNotificationPlatform.Apns, "some-device-id").Value, APP_ID, PushEnvironment.Development)
        };

        await connector.Send(registrations, new TestPushNotification { Data = "test-notification-payload" });

        // Assert
        client.SendAsyncCalls.Should().Be(1);
    }

    private static ApplePushNotificationServiceConnector CreateConnector(HttpClient httpClient)
    {
        var httpClientFactory = CreateHttpClientFactoryReturning(httpClient);
        var options = new OptionsWrapper<ApnsOptions>(new ApnsOptions
        {
            Keys = new Dictionary<string, ApnsOptions.Key>
            {
                {
                    "test-key-name", new ApnsOptions.Key
                    {
                        PrivateKey = "some-private-key",
                        TeamId = "some-team-id",
                        KeyId = "some-key-id"
                    }
                }
            },
            Bundles = new Dictionary<string, ApnsOptions.Bundle>
            {
                { APP_ID, new ApnsOptions.Bundle { KeyName = "test-key-name" } }
            }
        });
        var jwtGenerator = A.Dummy<IJwtGenerator>();
        var logger = A.Dummy<ILogger<ApplePushNotificationServiceConnector>>();
        var notificationTextProvider = A.Dummy<IPushNotificationTextProvider>();

        return new ApplePushNotificationServiceConnector(httpClientFactory, options, jwtGenerator, notificationTextProvider, logger);
    }

    private static IHttpClientFactory CreateHttpClientFactoryReturning(HttpClient client)
    {
        var httpClientFactory = A.Fake<IHttpClientFactory>();
        A.CallTo(() => httpClientFactory.CreateClient(A<string>._)).Returns(client);
        return httpClientFactory;
    }
}

public class HttpClientMock : HttpClient
{
    private readonly HttpMessageHandlerMock _handler;

    private HttpClientMock(HttpMessageHandlerMock handler) : base(handler)
    {
        _handler = handler;
    }

    public static HttpClientMock Create()
    {
        return new HttpClientMock(new HttpMessageHandlerMock());
    }

    public int SendAsyncCalls => _handler.SendAsyncCalls;

    private class HttpMessageHandlerMock : HttpMessageHandler
    {
        public int SendAsyncCalls;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref SendAsyncCalls);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}
