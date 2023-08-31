﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;

public class ApplePushNotificationServiceConnectorTests
{
    private const string APP_ID = "some-app-id";

    [Fact]
    public async Task Notification_is_sent_successfully()
    {
        // Arrange
        var client = HttpClientMock.Create();
        var connector = CreateConnector(client);

        // Act
        var recipient = IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j");
        var registrations = new List<PnsRegistration>
        {
            new(recipient, DeviceId.New(), PnsHandle.Parse("some-device-id", PushNotificationPlatform.Apns).Value, APP_ID)
        };
        await connector.Send(registrations, recipient, new { SomeProperty = "SomeValue" });

        // Assert
        client.SendAsyncCalls.Should().Be(1);
    }

    private static ApplePushNotificationServiceConnector CreateConnector(HttpClient httpClient)
    {
        var httpClientFactory = CreateHttpClientFactoryReturning(httpClient);
        var options = new OptionsWrapper<DirectPnsCommunicationOptions.ApnsOptions>(new DirectPnsCommunicationOptions.ApnsOptions()
        {
            Keys = new Dictionary<string, DirectPnsCommunicationOptions.ApnsOptions.Key>()
            {
                {"test-key-name", new DirectPnsCommunicationOptions.ApnsOptions.Key() {PrivateKey = "some-private-key"}}
            },
            Bundles = new Dictionary<string, DirectPnsCommunicationOptions.ApnsOptions.Bundle>()
            {
                {APP_ID, new DirectPnsCommunicationOptions.ApnsOptions.Bundle() { KeyName = "test-key-name", ServerType = DirectPnsCommunicationOptions.ApnsOptions.Bundle.ApnsServerType.Production }}
            }
        });
        var jwtGenerator = A.Fake<IJwtGenerator>();
        var logger = A.Fake<ILogger<ApplePushNotificationServiceConnector>>();

        return new ApplePushNotificationServiceConnector(httpClientFactory, options, jwtGenerator, logger);
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
