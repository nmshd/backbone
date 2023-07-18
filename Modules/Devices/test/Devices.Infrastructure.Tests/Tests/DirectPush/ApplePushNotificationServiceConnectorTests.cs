using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using static Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.IServiceCollectionExtensions.DirectPnsCommunicationOptions;
using DateTime = System.DateTime;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;

public class ApplePushNotificationServiceConnectorTests
{
    [Fact]
    public async Task Notification_is_sent_successfully()
    {
        // Arrange
        var httpClientFactory = A.Fake<IHttpClientFactory>();
        var clientHandlerStub = new DelegatingHandlerStub();
        var client = new HttpClient(clientHandlerStub);
        A.CallTo(() => httpClientFactory.CreateClient(A<string>._)).Returns(client);
        var options = A.Fake<IOptions<ApnsOptions>>();
        var logger = A.Fake<ILogger<ApplePushNotificationServiceConnector>>();
        var jwtGenerator = new MockJwtGenerator();
        var connector = new ApplePushNotificationServiceConnector(httpClientFactory, options, jwtGenerator, logger);
        var registrations = new List<PnsRegistration>()
        {
            new (IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), DeviceId.New(), PnsHandle.Parse("some-device-id", PushNotificationPlatform.Apns).Value)
        };
        var identityAddress = IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j");
        dynamic notification = new { Title = "", Body = "" };

        // Act
        await connector.Send(registrations, identityAddress, notification);

        // Assert
        clientHandlerStub.SendAsyncCalls.Should().Be(1);
    }

    [Fact]
    public async Task New_Token_is_generated_if_expiration_date_is_hit()
    {
        // Arrange
        var httpClientFactory = A.Fake<IHttpClientFactory>();
        var clientHandlerStub = new DelegatingHandlerStub();
        var client = new HttpClient(clientHandlerStub);
        A.CallTo(() => httpClientFactory.CreateClient(A<string>._)).Returns(client);
        var options = A.Fake<IOptions<ApnsOptions>>();
        var logger = A.Fake<ILogger<ApplePushNotificationServiceConnector>>();
        var jwtGenerator = new MockJwtGenerator();
        var connector = new ApplePushNotificationServiceConnector(httpClientFactory, options, jwtGenerator, logger);
        var registrations = new List<PnsRegistration>()
        {
            new (IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), DeviceId.New(), PnsHandle.Parse("some-device-id", PushNotificationPlatform.Apns).Value)
        };
        var identityAddress = IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j");
        dynamic notification = new { Title = "", Body = "" };

        // Act
        await connector.Send(registrations, identityAddress, notification);
        SystemTime.Set(DateTime.UtcNow.AddMinutes(50)); // Update System Time so that the token is considered expired
        await connector.Send(registrations, identityAddress, notification);

        // Assert
        MockJwtGenerator.NumberOfGeneratedTokens.Should().Be(2);
        clientHandlerStub.SendAsyncCalls.Should().Be(2);
    }

    [Fact]
    public async Task Token_is_only_generated_once_after_expiring_with_concurrent_notifications()
    {
        // Arrange
        var httpClientFactory = A.Fake<IHttpClientFactory>();
        var clientHandlerStub = new DelegatingHandlerStub();
        var client = new HttpClient(clientHandlerStub);
        A.CallTo(() => httpClientFactory.CreateClient(A<string>._)).Returns(client);
        var options = A.Fake<IOptions<ApnsOptions>>();
        var logger = A.Fake<ILogger<ApplePushNotificationServiceConnector>>();
        var jwtGenerator = new MockJwtGenerator();
        var connector = new ApplePushNotificationServiceConnector(httpClientFactory, options, jwtGenerator, logger);
        var registrations = new List<PnsRegistration>()
        {
            new (IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), DeviceId.New(), PnsHandle.Parse("some-device-id", PushNotificationPlatform.Apns).Value)
        };
        var identityAddress = IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j");
        dynamic notification = new { Title = "", Body = "" };

        await connector.Send(registrations, identityAddress, notification);
        SystemTime.Set(DateTime.UtcNow.AddMinutes(50)); // Update System Time so that the token is considered expired

        // Act
        var numberList = Enumerable.Range(1, 10000).ToList();
        var notificationTasks = numberList.Select(async _ =>
        {
            await connector.Send(registrations, identityAddress, notification);
        });
        await Task.WhenAll(notificationTasks);

        // Assert
        MockJwtGenerator.NumberOfGeneratedTokens.Should().Be(2);
        clientHandlerStub.SendAsyncCalls.Should().Be(10001);
    }

    [Fact]
    public async Task Token_is_only_generated_once_with_concurrent_notifications()
    {
        // Arrange
        var httpClientFactory = A.Fake<IHttpClientFactory>();
        var clientHandlerStub = new DelegatingHandlerStub();
        var client = new HttpClient(clientHandlerStub);
        A.CallTo(() => httpClientFactory.CreateClient(A<string>._)).Returns(client);
        var options = A.Fake<IOptions<ApnsOptions>>();
        var logger = A.Fake<ILogger<ApplePushNotificationServiceConnector>>();
        var jwtGenerator = new MockJwtGenerator();
        var connector = new ApplePushNotificationServiceConnector(httpClientFactory, options, jwtGenerator, logger);
        var registrations = new List<PnsRegistration>()
        {
            new (IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), DeviceId.New(), PnsHandle.Parse("some-device-id", PushNotificationPlatform.Apns).Value)
        };
        var identityAddress = IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j");
        dynamic notification = new { Title = "", Body = "" };

        // Act
        var numberList = Enumerable.Range(1, 10000).ToList();
        var notificationTasks = numberList.Select(async _ =>
        {
            await connector.Send(registrations, identityAddress, notification);
        });
        await Task.WhenAll(notificationTasks);

        // Assert
        MockJwtGenerator.NumberOfGeneratedTokens.Should().Be(1);
        clientHandlerStub.SendAsyncCalls.Should().Be(10000);
    }
}

public class DelegatingHandlerStub : DelegatingHandler
{
    public int SendAsyncCalls = 0;
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
    public DelegatingHandlerStub()
    {
        _handlerFunc = (request, _) => Task.FromResult(request.CreateResponse(HttpStatusCode.NoContent));
    }

    public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
    {
        _handlerFunc = handlerFunc;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        SendAsyncCalls++;
        return _handlerFunc(request, cancellationToken);
    }
}